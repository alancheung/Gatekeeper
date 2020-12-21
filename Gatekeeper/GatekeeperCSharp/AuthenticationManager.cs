using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace GatekeeperCSharp
{
    public class AuthenticationManager
    {
        /// <summary>
        /// The path to the authentication file.
        /// </summary>
        private const string AUTH_FILE_KEY = "authentication.json";

        private readonly string FILE_PATH = Path.Combine(Environment.CurrentDirectory, AUTH_FILE_KEY);

        /// <summary>
        /// A copy of the passwords saved into memory for faster verification
        /// </summary>
        private Password[] CachedPasswords;

        /// <summary>
        /// Constructor
        /// </summary>
        public AuthenticationManager()
        {
            Load();
        }

        /// <summary>
        /// Load the passwords from <see cref="FILE_PATH"/>
        /// </summary>
        /// <returns>All passwords read from <see cref="FILE_PATH"/></returns>
        public void Load()
        {
            string filePath = FILE_PATH;

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Authentication file does not exist!");
                CachedPasswords = new Password[0];
                return;
            }

            Password[] read = File.ReadAllLines(filePath)
                .Select(l => new Password(l))
                .ToArray();
            CachedPasswords = read;
        }

        /// <summary>
        /// Save the provided passwords in <paramref name="passwords"/> to <see cref="FILE_PATH"/>
        /// </summary>
        /// <param name="passwords">Passwords to save</param>
        public bool Save(string newPassword, out int id)
        {
            try
            {
                id = GetNextId();
                
                // Hash the provided passwords
                Password password = new Password(id, newPassword);

                // Now save the passwords to the file
                File.AppendAllLines(FILE_PATH, new string[1] { password.ToString() });

                // Refresh the cache since new password was added.
                Load();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Caught exception {ex.GetType()} during password save. Aborting!");
                id =  -1;
            }

            return id != -1;
        }

        /// <summary>
        /// Verify if the plain text <paramref name="plainText"/> is a known password.
        /// </summary>
        /// <param name="plainText">Plain text password to verify</param>
        /// <param name="id">Password ID if it was verified.</param>
        /// <returns>True if the password is authenticated, false otherwise.</returns>
        public bool Authenticate(string plainText, out string id)
        {
            // If this is the first run of the Authentication manager, then it should allow all access.
            if (CachedPasswords.Length == 0)
            {
                id = "NONE";
                return true;
            }
            else if (string.IsNullOrWhiteSpace(plainText))
            {
                id = "ERROR";
                return false;
            }
            {
                id = "UNKNOWN";
                bool found = false;
                foreach (Password password in CachedPasswords)
                {
                    found |= password.IsSame(plainText, out id);
                    if (found) break;
                }

                return found;
            }
        }

        /// <summary>
        /// Get the next maximum Id.
        /// </summary>
        /// <returns></returns>
        public int GetNextId()
        {
            if (CachedPasswords.Count() > 0)
            {
                return 1 + CachedPasswords.Length;
            }
            else
            {
                return 1;
            }
        }

        private class Password
        {
            /// <summary>
            /// <see cref="FORMAT"/> separator.
            /// </summary>
            private const char SEPARATOR = ':';

            /// <summary>
            /// The format that <see cref="Data"/>is saved to the file.
            /// {Id}:{Iterations}:{Password}
            /// </summary>
            private const string FORMAT = "{0}:{1}:{2}";

            /// <summary>
            /// Number of iterations to run the password hasher.
            /// </summary>
            private const int DefaultIterations = 10000;

            /// <summary>
            /// Number of bytes that make up the salt
            /// </summary>
            public const int SaltByteLength = 16;

            /// <summary>
            /// Number of bytes that make up the hashed password
            /// </summary>
            private const int HashByteLength = 32;

            /// <summary>
            /// The data saved to the file.
            /// </summary>
            public string Data { get; set; }

            /// <summary>
            /// Id of the <see cref="Data"/> password.
            /// </summary>
            public string PasswordId { get; set; }

            /// <summary>
            /// Number of iterations required to generate the currect <see cref="Data"/>.
            /// </summary>
            public int Iterations { get; set; }

            /// <summary>
            /// Constructor given an already hashed password.
            /// </summary>
            /// <param name="data">File password data to deconstruct</param>
            public Password(string data)
            {
                // Decode the salt/hash from the saved password
                string[] splitData = data?.Split(SEPARATOR);

                if (string.IsNullOrWhiteSpace(data)
                    || string.IsNullOrWhiteSpace(splitData[0])
                    || !int.TryParse(splitData[1], out int iterations)
                    || string.IsNullOrWhiteSpace(splitData[2]))
                {
                    throw new InvalidDataException($"Initialized password data was not in a valid format!");
                }

                Data = data;
                PasswordId = splitData[0];
                Iterations = iterations;
            }

            /// <summary>
            /// Password given a plain text password that should be hashed.
            /// </summary>
            /// <param name="id">ID of the new password</param>
            /// <param name="plainText">The plain text password to hash</param>
            public Password(int id, string plainText)
            {
                string hashed = Hash(plainText);
                Data = string.Format(FORMAT, id, DefaultIterations, hashed);
            }

            /// <summary>
            /// Check that the password hashed in <see cref="Data"/> is equal to the hashed value of <paramref name="plainText"/>
            /// </summary>
            /// <param name="plainText">Password to hash and verify against</param>
            /// <param name="id">If the password is the same, ID of the password.</param>
            /// <returns>True if the <paramref name="plainText"/> hashes to the data in <see cref="Data"/></returns>
            public bool IsSame(string plainText, out string id)
            {
                // Get hash bytes
                byte[] hashedPassword = Convert.FromBase64String(Data.Split(':')[2]);

                // Get salt
                byte[] salt = new byte[SaltByteLength];
                byte[] hash = new byte[HashByteLength];
                Array.Copy(hashedPassword, 0, salt, 0, SaltByteLength);
                Array.Copy(hashedPassword, SaltByteLength, hash, 0, HashByteLength);

                // Create hash of plain text password with given salt
                Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(plainText, salt, Iterations);
                byte[] plainTextHash = pbkdf2.GetBytes(HashByteLength);

                id = PasswordId;
                return ByteCompare(hash, plainTextHash);
            }

            /// <summary>
            /// Hash the <paramref name="plainText"/>.
            /// </summary>
            /// <param name="plainText">Plain text password to hash.</param>
            /// <returns>Base64 hashed password.</returns>
            private string Hash(string plainText)
            {
                // Create salt bytes
                byte[] salt = new byte[SaltByteLength];
                new RNGCryptoServiceProvider().GetBytes(salt);

                // Create hashed password
                Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(plainText, salt, DefaultIterations);
                byte[] hash = pbkdf2.GetBytes(HashByteLength);

                // Combine salt with the hash
                // [0 => SaltBytes, SaltBytes => SaltBytes + HashBytes]
                byte[] hashedPasswordBytes = new byte[SaltByteLength + HashByteLength];
                Array.Copy(salt, 0, hashedPasswordBytes, 0, SaltByteLength);
                Array.Copy(hash, 0, hashedPasswordBytes, SaltByteLength, HashByteLength);

                // Save salt and hash as Base64
                string hashPassword = Convert.ToBase64String(hashedPasswordBytes);
                return hashPassword;
            }

            /// <summary>
            /// Compare 2 byte array sequences.
            /// </summary>
            /// <param name="first">First byte array</param>
            /// <param name="second">Second byte array</param>
            /// <returns>True if <paramref name="first"/> and <paramref name="second"/> are equal.</returns>
            private bool ByteCompare(byte[] first, byte[] second)
            {
                try
                {
                    if (first.Count() != second.Count())
                    {
                        throw new Exception($"Source collection length ({first.Count()}) not equal to target collection length ({second.Count()})!");
                    }

                    for (int c = 0; c < first.Count(); c++)
                    {
                        if (first[c] != second[c])
                        {
                            throw new Exception($"Byte comparison at index {c} is not equal!");
                        }
                    }

                    return true;

                }
                catch (Exception)
                {
                    return false;
                }
            }

            /// <summary>
            /// Override of ToString
            /// </summary>
            /// <returns>String</returns>
            public override string ToString()
            {
                return Data.ToString();
            }
        }
    }
}
