using GatekeeperCSharp.Common;
using GatekeeperCSharp.GPIO;
using System;
using System.Drawing;
using System.Windows.Forms;
using Unosquare.RaspberryIO.Abstractions;

namespace GatekeeperCSharp
{
    public partial class GatekeeperForm : Form
    {
        /// <summary>
        /// Size of the RaspberryPi 7" touchscreen.
        /// </summary>
        public static readonly Size ScreenSize = new Size(800, 480);

        /// <summary>
        /// Should this FORM be run in RELEASE mode.
        /// </summary>
        private readonly bool RELEASE = Program.RELEASE;

        /// <summary>
        /// Manage all authentication requests and workflows.
        /// </summary>
        private readonly AuthenticationManager _authManager;

        /// <summary>
        /// Control over GPIO.
        /// </summary>
        private readonly IGpioManager _gpio;

        /// <summary>
        /// GPIO BCM pin that controls the relay.
        /// </summary>
        private readonly BcmPin _relayPin;

        /// <summary>
        /// Amount of time in milliseconds that the lock should be held open for.
        /// </summary>
        private readonly TimeSpan _openTime;

        /// <summary>
        /// Pass-through control to <see cref="StatusLabel.Text"/> property.
        /// </summary>
        public string Status
        {
            get => StatusLabel.Text;
            set
            {
                StatusLabel.Text = value;
                StatusLabel.Update();
            }
        }

        /// <summary>
        /// The string constructed from the numerical password input.
        /// </summary>
        public string Input { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="authentication"></param>
        /// <param name="gpioManager"></param>
        /// <param name="relayPin"></param>
        /// <param name="openTime"></param>
        public GatekeeperForm(AuthenticationManager authentication, IGpioManager gpioManager, BcmPin relayPin, TimeSpan openTime)
        {
            InitializeComponent();
            InitializeFormHeader();

            _authManager = authentication;
            _gpio = gpioManager;
            _relayPin = relayPin;
            _openTime = openTime;
            _gpio.OnRfidCardDetected += gpio_OnRfidCardDetected;

            Clear();
        }

        /// <summary>
        /// Event triggered when the <see cref="IGpioManager"/> detects a new RFID card.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments included in RFID detection</param>
        private void gpio_OnRfidCardDetected(object sender, RfidDetectedEventArgs e)
        {
            string cardData = e.Data.Stringify(d => d, string.Empty);
            if (_authManager.Authenticate(cardData, out string id))
            {
                Status = "ACCESS GRANTED";
                ToggleLock();
                Clear();
            }
            else
            {
                Clear();
                Status = "ACCESS DENIED";
            }
        }

        /// <summary>
        /// Event triggered when the <see cref="IGpioManager"/> detects a new RFID card.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments included in RFID detection</param>
        private void SaveNewCardOnRfidCardDetected(object sender, RfidDetectedEventArgs e)
        {
            string cardData = e.Data.Stringify(d => d, string.Empty);
            if (_authManager.Save(Input, out int newPasswordId))
            {
                Status = $"Password Id {newPasswordId} Saved!";

                // Reset the listeners automatically.
                Admin_AddCardButton_Click(null, null);
            }
            else
            {
                Status = $"Failed to Save!";
            }
        }

        /// <summary>
        /// Initialize the form header by setting title and sizing.
        /// </summary>
        private void InitializeFormHeader()
        {
            ClientSize = ScreenSize;
            FormBorderStyle = FormBorderStyle.None;

            RightTablePanel.Location = new Point(ScreenSize.Width / 2, 0);
            RightTablePanel.Size = new Size(ScreenSize.Width / 2, ScreenSize.Height);

            AdminTablePanel.Location = new Point(0, 0);
            AdminTablePanel.Size = new Size(ScreenSize.Width / 2, ScreenSize.Height);

            if (RELEASE)
            {
                Text = "Gatekeeper";
            }
            else
            {
                Text = "Gatekeeper - DEBUG";
            }
        }

        /// <summary>
        /// Toggle the magnetic lock relay open.
        /// </summary>
        private void ToggleLock()
        {
            _gpio.Toggle(_relayPin, GpioPinValue.High, _openTime);
        }

        #region Form Listeners
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumberButton_Click(object sender, EventArgs e)
        {
            Input += ((Button)sender).Text;

            Status = Input.Obfuscate();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            Input = string.Empty;
            Status = string.Empty;
        }

        private void AdminButton_Click(object sender, EventArgs e)
        {
            // If the admin panel is being displayed, check the password
            if (!AdminTablePanel.Visible)
            {
                if (_authManager.Authenticate(Input, out string id))
                {
                    Clear();
                    Status = "Welcome Admin";
                    AdminTablePanel.Visible = true;
                }
                else
                {
                    Clear();
                    Status = "ACCESS DENIED";
                }
            }
            else
            {
                Clear();
                AdminTablePanel.Visible = false;
            }

            SubmitButton.Enabled = !AdminTablePanel.Visible;
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            if (_authManager.Authenticate(Input, out string id))
            {
                Status = "ACCESS GRANTED";
                ToggleLock();
                ClearButton_Click(null, null);
            }
            else
            {
                Clear();
                Status = "ACCESS DENIED";
            }
        }
        #endregion

        #region Admin Button Listeners
        private void Admin_DebugButton_Click(object sender, EventArgs e)
        {

        }

        private void Admin_ExitButton_Click(object sender, EventArgs e)
        {
            Clear();

            Status = "Stopping...";
            (_gpio as IDisposable)?.Dispose();

            Close();
        }

        private void Admin_AddNewPasswordButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Input))
            {
                Status = "Please Enter a Password!";
            }
            else
            {
                if (_authManager.Save(Input, out int newPasswordId))
                {
                    Status = $"Password Id {newPasswordId} Saved!";
                }
                else
                {
                    Status = $"Failed to Save!";
                }
            }
        }

        private void Admin_LoadPasswordButton_Click(object sender, EventArgs e)
        {
            _authManager.Load();
        }

        private void Admin_TriggerRfidButton_Click(object sender, EventArgs e)
        {
            (_gpio as GpioManagerSimulator)?.InvokeOnRfidCardDetected();
        }

        private bool savingNewCard = false;

        private void Admin_AddCardButton_Click(object sender, EventArgs e)
        {
            if (!savingNewCard)
            {
                // Unsubscribe real listener
                _gpio.OnRfidCardDetected -= gpio_OnRfidCardDetected;

                // Start listening for new cards
                _gpio.OnRfidCardDetected += SaveNewCardOnRfidCardDetected;

                savingNewCard = true;
            }
            else
            {
                // Stop listening new cards
                _gpio.OnRfidCardDetected -= SaveNewCardOnRfidCardDetected;

                // Start listening for real again
                _gpio.OnRfidCardDetected += gpio_OnRfidCardDetected;

                savingNewCard = false;
            }

            AddCardButton.BackColor = savingNewCard ? Color.Green : Color.Transparent;
        }
        #endregion
    }
}
