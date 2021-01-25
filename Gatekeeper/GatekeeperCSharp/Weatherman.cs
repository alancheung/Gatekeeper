using GatekeeperCSharp.Key;
using System;
using System.Drawing;
using System.IO;
using System.Timers;
using WeatherNet.Clients;
using WeatherNet.Model;

namespace GatekeeperCSharp
{
    public class UIWeatherUpdate : EventArgs
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
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
        public EventHandler<SingleResult<CurrentWeatherResult>> OnCurrentWeatherUpdate;

        /// <summary>
        /// Path the assets folder.
        /// </summary>
        public static string ImageFolderPath = Path.Combine(Environment.CurrentDirectory, "Assets");

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">API key for OpenWeather</param>
        public Weatherman(string key)
        {
            WeatherNet.ClientSettings.SetApiKey(key);
        }

        /// <summary>
        /// Initializes and sets up the timer for regular updates.
        /// </summary>
        /// <param name="interval"></param>
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
        /// <returns></returns>
        public bool UpdateWeather()
        {
            SingleResult<CurrentWeatherResult> result = CurrentWeather.GetByCoordinates(APIKeys.Latitude, APIKeys.Longitude, "en", "imperial");
            if (result.Success)
            {
                LastUpdate = DateTimeOffset.Now;
                _currentWeather = result.Item;
            }
            else
            {
                Console.WriteLine($"Weather update failed with message: {result.Message}");
            }

            OnCurrentWeatherUpdate?.Invoke(this, result);
            return result.Success;
        }

        public Image CreateImage(string icon)
        {
            return new Bitmap(Path.Combine(ImageFolderPath, icon));
        }
    }
}
