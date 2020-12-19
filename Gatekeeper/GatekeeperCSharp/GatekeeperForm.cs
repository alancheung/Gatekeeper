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
        /// Should this FORM be run in RELEASE mode.
        /// </summary>
        private readonly bool RELEASE = Program.RELEASE;


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

            ClearButton_Click(this, null);
        }

        /// <summary>
        /// Initialize the form header by setting title and sizing.
        /// </summary>
        private void InitializeFormHeader()
        {
            ClientSize = new Size(800, 480);
            FormBorderStyle = FormBorderStyle.None;

            if (RELEASE)
            {
                Text = "Gatekeeper";
                StartPosition = FormStartPosition.CenterScreen;
            }
            else
            {
                Text = "Gatekeeper - DEBUG";
            }
        }

        #region Form Listeners
        private void AdminButton_Click(object sender, EventArgs e)
        {

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
            ClearButton_Click(sender, e);
        }
        #endregion
    }
}
