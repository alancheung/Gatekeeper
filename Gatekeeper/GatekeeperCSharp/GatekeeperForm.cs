using GatekeeperCSharp.Common;
using GatekeeperCSharp.GPIO;
using GatekeeperCSharp.Secrets;
using Raspi.Communication;
using Raspi.Communication.Messages;
using Swan.Formatters;
using System;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
        /// Manage weather related requests.
        /// </summary>
        private readonly Weatherman _ollieWilliams;

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
        /// Disable unlocking.
        /// </summary>
        private bool _lockDisabled;

        /// <summary>
        /// Pass-through control to <see cref="StatusLabel.Text"/> property.
        /// </summary>
        public string Status
        {
            get => StatusLabel.Text;
            set
            {
                StatusLabel.SetText(value);
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
        /// <param name="ollieWilliams"></param>
        /// <param name="gpioManager"></param>
        /// <param name="communicationClient"
        /// <param name="relayPin"></param>
        /// <param name="openTime"></param>
        public GatekeeperForm(AuthenticationManager authentication, Weatherman ollieWilliams, IGpioManager gpioManager, UdpReceiver communicationClient, BcmPin relayPin, TimeSpan openTime)
        {
            InitializeComponent();

            _lockDisabled = false;

            _authManager = authentication;
            _ollieWilliams = ollieWilliams;
            _gpio = gpioManager;
            _relayPin = relayPin;
            _openTime = openTime;

            _ollieWilliams.OnWeatherUpdate += _ollieWilliams_OnWeatherUpdate;
            _ollieWilliams.OnForecastUpdate += _ollieWilliams_OnForecastUpdate;
            _gpio.OnRfidCardDetected += gpio_OnRfidCardDetected;
            _gpio.OnValidDhtData += _gpio_OnValidDhtData;
            communicationClient.OnCommunicationData += CommunicationClient_OnCommunicationData;

            InitializeFormHeader();
            Clear();

            // First load
            _ollieWilliams.UpdateWeather();
            _ollieWilliams.UpdateForecast();
        }

        private void CommunicationClient_OnCommunicationData(object sender, ICommand cmd)
        {
            if (cmd is LockCommand)
            {
                bool lockState = (cmd as LockCommand).Lock;
                Status = lockState ? "Disabled..." : string.Empty;
                _lockDisabled = lockState;
            }
        }

        private void _gpio_OnValidDhtData(object sender, DhtEventArgs e)
        {
            string title = $"Temperature: {e.Temperature:0.00}°F /// Humidity: {e.Humidity}%";
            WeatherTitleLabel.SetText(title);
            
            if (e.Humidity > 60d)
            {
                Status = "Humidity Warning";
            }
        }

        /// <summary>
        /// Event triggered when <see cref="Weatherman"/> has a current weather update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="update"></param>
        private void _ollieWilliams_OnWeatherUpdate(object sender, UIWeatherUpdate update)
        {
            CurrentWeatherIcon.ImageLocation = update.IconPath;
            CurrentWeatherLabel.SetText(update.Description);
            LastWeatherUpdateLabel.SetText(update.LastUpdateMessage);
        }

        /// <summary>
        /// Event triggered when <see cref="Weatherman"/> has a forecast update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="update"></param>
        private void _ollieWilliams_OnForecastUpdate(object sender, UIWeatherUpdate[] updates)
        {
            UIWeatherUpdate tomorrow = updates.FirstOrDefault(u => u.TimeStamp.Date == DateTimeOffset.Now.AddDays(1).Date);
            SecondWeatherIcon.ImageLocation = tomorrow?.IconPath;
            SecondWeatherLabel.SetText(tomorrow?.Description);

            UIWeatherUpdate threeDay = updates.FirstOrDefault(u => u.TimeStamp.Date == DateTimeOffset.Now.AddDays(2).Date);
            ThirdWeatherIcon.ImageLocation = threeDay?.IconPath;
            ThirdWeatherLabel.SetText(threeDay?.Description);
        }

        /// <summary>
        /// Event triggered when the <see cref="IGpioManager"/> detects a new RFID card.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments included in RFID detection</param>
        private async void gpio_OnRfidCardDetected(object sender, RfidDetectedEventArgs e)
        {
            string cardData = e.Data.Stringify(d => d, string.Empty);
            if (_authManager.Authenticate(cardData, out string id))
            {
                Status = $"Welcome {id}!";
                await ToggleLock();
                Clear();
            }
            else
            {
                Clear();
                Status = "ACCESS DENIED";
            }
        }

        /// <summary>
        /// Event triggered when the <see cref="IGpioManager"/> detects a new RFID card. Just displays the unencrypted card details
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Arguments included in RFID detection</param>
        private void SaveNewCardOnRfidCardDetected(object sender, RfidDetectedEventArgs e)
        {
            string cardData = e.Data.Stringify(d => d, string.Empty);
            Input = cardData;
            Status = cardData;
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

            InformationPanel.Location = new Point(0, 0);
            InformationPanel.Size = new Size(ScreenSize.Width / 2, ScreenSize.Height);
            WeatherTitleLabel.Size = new Size((ScreenSize.Width / 2) - 1, 50);

            if (RELEASE)
            {
                Text = "Gatekeeper";
            }
            else
            {
                Text = "Gatekeeper - DEBUG";
            }

            WeatherSourceButton.SetText($"Use {(_ollieWilliams.UseRealWeather ? "Fake" : "Real")} Weather");
        }

        /// <summary>
        /// Toggle the magnetic lock relay open.
        /// </summary>
        private async Task ToggleLock()
        {
            await _gpio.Toggle(_relayPin, GpioPinValue.High, _openTime);
        }

        #region Keypad Form Listeners
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
                    Status = $"Hello {id}";
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
            InformationPanel.Visible = !AdminTablePanel.Visible;
        }

        private async void SubmitButton_Click(object sender, EventArgs e)
        {
            if (_lockDisabled)
            {
                ClearButton_Click(null, null);
                Status = "Lock Disabled!";
                return;
            }

            if (_authManager.Authenticate(Input, out string id))
            {
                Status = $"Welcome {id}!";
                Task lockTask = ToggleLock();
                Task wakeTask = WOL.Send(SecretKeys.DesktopMAC);

                await Task.WhenAll(lockTask, wakeTask);
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
            (_gpio as GpioManagerSimulator)?.InvokeOnValidDhtData();
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

        private void Admin_AddCardButton_Click(object sender, EventArgs e)
        {
            if (!_gpio.AddingNewRfidCard)
            {
                Clear();

                // Unsubscribe real listener
                _gpio.OnRfidCardDetected -= gpio_OnRfidCardDetected;

                // Start listening for new cards
                _gpio.OnRfidCardDetected += SaveNewCardOnRfidCardDetected;

                _gpio.AddingNewRfidCard = true;

                Status = "Waiting...";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Input))
                {
                    Status = "Nothing Saved!";
                }
                else if (_authManager.Save(Input, out int newPasswordId))
                {
                    Clear();
                    Status = $"Password Id {newPasswordId} Saved!";
                }
                else
                {
                    Status = "Failed to Save!";
                }

                // Stop listening for new cards
                _gpio.OnRfidCardDetected -= SaveNewCardOnRfidCardDetected;

                // Start listening for real again
                _gpio.OnRfidCardDetected += gpio_OnRfidCardDetected;

                _gpio.AddingNewRfidCard = false;
            }

            AddCardButton.BackColor = _gpio.AddingNewRfidCard ? Color.Green : Color.Transparent;
        }

        private async void Admin_TurnOffLightsButton_Click(object sender, EventArgs e)
        {
            string server = ConfigurationManager.AppSettings["server"];
            string lifxApiAddress = Uri.EscapeUriString($"{server}/api/lifx");

            string[] lights = ConfigurationManager.AppSettings["lights"].Split(',');
            object requestObj = new
            {
                Lights = lights,
                TurnOff = true,
                Duration = 1000,
            };
            StringContent content = new StringContent(Json.Serialize(requestObj), Encoding.UTF8, "application/json");

            await Task.Run(async () =>
            {
                try
                {
                    HttpClient client = new HttpClient();
                    HttpResponseMessage response = await client.PostAsync(lifxApiAddress, content);
                    Status = "During";
                    Console.WriteLine("During");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Light request exception handled.");
                    Console.WriteLine($"Exception: {ex}");
                }
            });
        }

        private void Admin_ToggleLockButton_Click(object sender, EventArgs e)
        {
            _lockDisabled = !_lockDisabled;
            Status = _lockDisabled ? "Disabled..." : "Enabled...";
        }

        private void Admin_WeatherSourceButton_Click(object sender, EventArgs e)
        {
            _ollieWilliams.UseRealWeather = !_ollieWilliams.UseRealWeather;
            WeatherSourceButton.SetText($"Use {(_ollieWilliams.UseRealWeather ? "Fake" : "Real")} Weather");
        }

        private void Admin_UpdateWeatherButton_Click(object sender, EventArgs e)
        {
            _ollieWilliams.UpdateWeather();
            _ollieWilliams.UpdateForecast();
        }
        #endregion

        #region Weather Button Listeners
        private void CurrentWeatherTitleLabel_Click(object sender, EventArgs e)
        {

        }

        private void CurrentWeather_Click(object sender, EventArgs e)
        {

        }

        private void SecondWeather_Click(object sender, EventArgs e)
        {

        }

        private void ThirdWeather_Click(object sender, EventArgs e)
        {

        }
        #endregion

        
    }
}
