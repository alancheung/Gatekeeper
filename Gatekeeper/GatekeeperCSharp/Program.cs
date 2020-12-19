using GatekeeperCSharp.GPIO;
using GatekeeperCSharp.Simulate;
using System;
using System.Configuration;
using System.Windows.Forms;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace GatekeeperCSharp
{
    public static class Program
    {

#if DEBUG
        /// <summary>
        /// Is the current program executing in RELEASE mode? Determined with !DEBUG directive.
        /// Static readonly to prevent unreachable code warnings.
        /// </summary>
        public static readonly bool RELEASE = false;
#else
        public static readonly bool RELEASE = true;
#endif

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // References required in finally
            IGpioManager gpio = null;
            BcmPin? relayPin = null;

            try
            {
                // Read configuration values
                relayPin = (BcmPin)int.Parse(ConfigurationManager.AppSettings["relay_pin"]);
                TimeSpan openTime = TimeSpan.FromMilliseconds(double.Parse(ConfigurationManager.AppSettings["open_time"]));

                // Initialize gpio for simulated or real runs.
                if (RELEASE)
                {
                    gpio = new GpioManager();
                }
                else
                {
                    gpio = new GpioManagerSimulator();
                }

                // Initialize application settings.
                gpio.Initialize(relayPin.Value, GpioPinDriveMode.Output);
                
                // C# Start
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Form form = new GatekeeperForm(gpio, relayPin.Value, openTime);
                Application.Run(form);
            }
            finally
            {
                if (gpio != null && relayPin.HasValue)
                {
                    gpio.Toggle(relayPin.Value, GpioPinValue.High, TimeSpan.FromSeconds(3));
                }
            }
        }
    }
}
