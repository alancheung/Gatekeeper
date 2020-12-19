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

        /// <summary>
        /// GPIO BCM pin that controls the relay.
        /// </summary>
        private readonly BcmPin _relayPin;

        /// <summary>
        /// Amount of time in milliseconds that the lock should be held open for.
        /// </summary>
        private readonly TimeSpan _openTime;

        private readonly IGpioManager _gpio;

        public GatekeeperForm(IGpioManager gpioManager, BcmPin relayPin, TimeSpan openTime)
        {
            InitializeComponent();
            InitializeFormHeader();

            _gpio = gpioManager;
            _relayPin = relayPin;
            _openTime = openTime;

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

        private void button1_Click(object sender, EventArgs e)
        {
            _gpio.Toggle(_relayPin, GpioPinValue.High, _openTime);
        }
    }
}
