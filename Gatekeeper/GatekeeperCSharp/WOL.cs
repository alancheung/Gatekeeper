using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GatekeeperCSharp
{
    /// <summary>
    /// Credit to https://stackoverflow.com/a/65529803/11947191
    /// </summary>
    public static class WOL
    {
        /// <summary>
        /// Broadcast address
        /// </summary>
        private static readonly IPEndPoint BroadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, 9);

        /// <summary>
        /// Send a Wake On Lan (WOL) request to the specific MAC address represented by <paramref name="macAddress"/>
        /// </summary>
        /// <param name="macAddress">The target machine to wakeup.</param>
        /// <returns>A <see cref="UdpClient.SendAsync(byte[], int, IPEndPoint)"/> result.</returns>
        public static async Task Send(string macAddress)
        {
            byte[] magicPacket = BuildMagicPacket(macAddress);

            using (UdpClient client = new UdpClient())
            {
                await client.SendAsync(magicPacket, magicPacket.Length, BroadcastEndpoint);
            }
        }

        private static byte[] BuildMagicPacket(string macAddress) // MacAddress in any standard HEX format
        {
            macAddress = Regex.Replace(macAddress, "[: -]", "");
            byte[] macBytes = new byte[6];
            for (int i = 0; i < 6; i++)
            {
                macBytes[i] = Convert.ToByte(macAddress.Substring(i * 2, 2), 16);
            }

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    for (int i = 0; i < 6; i++)  //First 6 times 0xff
                    {
                        bw.Write((byte)0xff);
                    }
                    for (int i = 0; i < 16; i++) // then 16 times MacAddress
                    {
                        bw.Write(macBytes);
                    }
                }
                return ms.ToArray(); // 102 bytes magic packet
            }
        }
    }
}
