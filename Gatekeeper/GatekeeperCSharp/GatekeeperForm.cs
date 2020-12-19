using GatekeeperCSharp.Common;
using RaspiSimulator.GPIO;
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
            }
        }

        public string Input { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gpioManager"></param>
        /// <param name="relayPin"></param>
        /// <param name="openTime"></param>
        public GatekeeperForm(IGpioManager gpioManager, BcmPin relayPin, TimeSpan openTime)
        {
            InitializeComponent();
            InitializeFormHeader();

            _gpio = gpioManager;
            _relayPin = relayPin;
            _openTime = openTime;
            _gpio.OnRfidCardDetected += gpio_OnRfidCardDetected;

            ClearButton_Click(this, null);
        }

        /// <summary>
        /// Event triggered when the <see cref="IGpioManager"/> detects a new RFID card.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments included in RFID detection</param>
        private void gpio_OnRfidCardDetected(object sender, RfidDetectedEventArgs e)
        {
            Status = $"CARD: {e.Data.Stringify(d => d)}";
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

        #region Form Listeners
        private void AdminButton_Click(object sender, EventArgs e)
        {
            AdminTablePanel.Visible = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumberButton_Click(object sender, EventArgs e)
        {
            Input += ((Button)sender).Text;

            Status = Input.Obfuscate();
            Console.WriteLine(Input);
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            Input = string.Empty;
            Status = string.Empty;
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            _gpio.Toggle(_relayPin, GpioPinValue.High, _openTime);
        }
        #endregion

        #region Admin Button Listeners
        private void Admin_DebugButton_Click(object sender, EventArgs e)
        {

        }

        private void Admin_ExitButton_Click(object sender, EventArgs e)
        {
            ClearButton_Click(this, e);

            Status = "Stopping...";
            (_gpio as IDisposable)?.Dispose();

            Close();
        }
        #endregion
    }
}
