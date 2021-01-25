using GatekeeperCSharp.Key;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Timers;
using WeatherNet.Clients;
using WeatherNet.Model;

namespace GatekeeperCSharp
{
    /// <summary>
    /// Simple weather update to give to UI to display. All logic should be handled here.
    /// </summary>
    public class UIWeatherUpdate : EventArgs
    {
        public UIWeatherUpdate(SingleResult<CurrentWeatherResult> update)
        {
            Title = $"{update.Item.Title} - {update.Item.Description}";

            StringBuilder weatherBuilder = new StringBuilder();
            weatherBuilder.AppendLine($"{update.Item.Title} - {update.Item.Description}");
            weatherBuilder.AppendLine($"Current: {update.Item.Temp}°F");
            weatherBuilder.AppendLine($"Forecast: {update.Item.TempMin}°F / {update.Item.TempMax}°F");
            weatherBuilder.AppendLine($"Humidity: {update.Item.Humidity}%");
            Description = weatherBuilder.ToString();

            string icon = Path.Combine(Weatherman.ImageFolderPath, update.Item.Icon);
            icon = Path.ChangeExtension(icon, "png");
            IconPath = icon;
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public string IconPath { get; set; }
        public string LastUpdate { get; set; }
    }

    public class Weatherman
    {
        /// <summary>
        /// Timestamp of last weather update.
        /// </summary>
        public DateTimeOffset LastUpdate;

        /// <summary>
        /// Calculated time of the next update. This is not exact due to timer limitations.
        /// </summary>
        public DateTimeOffset NextUpdate => LastUpdate != default && _timer != null
            ? LastUpdate.Add(TimeSpan.FromMilliseconds(_timer.Interval))
            : DateTimeOffset.MinValue;

        /// <summary>
        /// Internal timer to schedule regular UI updates
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// The cached results of the weather update.
        /// </summary>
        private CurrentWeatherResult _currentWeather;

        /// <summary>
        /// Event fired when the weather has been updated.
        /// </summary>
        public EventHandler<UIWeatherUpdate> OnCurrentWeatherUpdate;

        /// <summary>
        /// Path the assets folder.
        /// </summary>
        public static string ImageFolderPath = Path.Combine(Environment.CurrentDirectory, "Assets");

        /// <summary>
        /// Should we get the real weather from the source?
        /// </summary>
        public bool UseRealWeather { get; set; }

        /// <summary>
        /// Did the last update fail?
        /// </summary>
        public bool LastUpdateFailed { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">API key for OpenWeather</param>
        /// <param name="useRealWeather">Use the real weather.</param>
        public Weatherman(string key, bool useRealWeather = false)
        {
            WeatherNet.ClientSettings.SetApiKey(key);
            UseRealWeather = useRealWeather;
            LastUpdateFailed = false;
        }

        /// <summary>
        /// Initializes and sets up the timer for regular updates.
        /// </summary>
        /// <param name="interval">The time interval until the next update.</param>
        public void SetUpdateInterval(TimeSpan interval)
        {
            if (interval.TotalMilliseconds > 0)
            {
                if (_timer == null)
                {
                    _timer = new Timer(interval.TotalMilliseconds);
                    _timer.AutoReset = true;
                    _timer.Enabled = true;
                    _timer.Elapsed += (object sender, ElapsedEventArgs e) => UpdateWeather();
                }

                _timer.Stop();
                _timer.Interval = interval.TotalMilliseconds;
                _timer.Start();
            }
            else
            {
                _timer.Stop();
                _timer?.Dispose();
                _timer = null;
            }
        }

        /// <summary>
        /// Update the weather held in memory and broadcast the updates.
        /// </summary>
        /// <returns>Was the update successful</returns>
        public bool UpdateWeather()
        {
            SingleResult<CurrentWeatherResult> result;
            if (UseRealWeather)
            {
                result = CurrentWeather.GetByCoordinates(APIKeys.Latitude, APIKeys.Longitude, "en", "imperial");
            }
            else
            {
                string debugWeatherFilePath = Path.Combine(Environment.CurrentDirectory, "fakeWeather.json");
                string fakeWeatherJson = File.ReadAllText(debugWeatherFilePath);
                result = JsonConvert.DeserializeObject<SingleResult<CurrentWeatherResult>>(fakeWeatherJson);
                Console.WriteLine("Faked weather results!");
            }
            
            if (result.Success)
            {
                LastUpdate = DateTimeOffset.Now;
                LastUpdateFailed = false;
                _currentWeather = result.Item;
            }
            else
            {
                LastUpdateFailed = true;
                Console.WriteLine($"Weather update failed with message: {result.Message}");
            }

            UIWeatherUpdate forUi = new UIWeatherUpdate(result);
            forUi.LastUpdate = $"Last Updated: {LastUpdate.ToString("HH:mm")} - Next: {NextUpdate.ToString("HH:mm")}";

            OnCurrentWeatherUpdate?.Invoke(this, forUi);
            return result.Success;
        }
    }
}
