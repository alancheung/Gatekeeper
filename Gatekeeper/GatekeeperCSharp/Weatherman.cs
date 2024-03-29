﻿using GatekeeperCSharp.Secrets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public UIWeatherUpdate(WeatherResult update, bool successful)
        {
            bool isCurrent = update is CurrentWeatherResult;

            Result = update;
            TimeStamp = update.Date;
            Title = $"{update.Title} - {update.Description}";

            StringBuilder weatherBuilder = new StringBuilder();
            weatherBuilder.AppendLine($"{update.Title} - {update.Description}");
            weatherBuilder.AppendLine($"Time: {TimeStamp.ToString("M/d/yy @ HH:mm")}");
            weatherBuilder.AppendLine($"Wind: {update.WindSpeed}");
            if (isCurrent)
            {
                weatherBuilder.AppendLine($"Current: {update.Temp:0.00}°F");
            }
            else
            {
                weatherBuilder.AppendLine($"Min/Max: {Math.Round(update.TempMin)}°F / {Math.Round(update.TempMax)}°F");
            }
            weatherBuilder.AppendLine($"Humidity: {update.Humidity}%");
            Description = weatherBuilder.ToString();

            string icon = Path.Combine(Weatherman.ImageFolderPath, update.Icon);
            icon = Path.ChangeExtension(icon, "png");
            IconPath = icon;

            NewData = successful;
            DateTimeOffset now = DateTimeOffset.Now;
            string updateType = string.Empty;
            if (!NewData)
            {
                updateType = " (FAILED)";
            }
            else
            {
                updateType = $" (from {TimeStamp.ToString("M/d/yy@HH:mm")})";
            }

            LastUpdateMessage = $"Last Update{updateType}: {now.ToString("HH:mm")}";
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public string IconPath { get; set; }
        public string LastUpdateMessage { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public WeatherResult Result { get; set; }
        public bool NewData { get; set; }
    }

    public class Weatherman
    {
        /// <summary>
        /// Timestamp of last weather update.
        /// </summary>
        public DateTimeOffset LastUpdate;

        /// <summary>
        /// Internal timer to schedule regular UI updates for the current weather
        /// </summary>
        private Timer _currentWeatherTimer;

        /// <summary>
        /// 
        /// Internal timer to schedule regular UI updates for forecasted weather.
        /// </summary>
        private Timer _forecastWeatherTimer;

        /// <summary>
        /// The cached results of the weather update.
        /// </summary>
        private SingleResult<CurrentWeatherResult> _currentWeather;

        /// <summary>
        /// The cached results of the weather forecast.
        /// </summary>
        private Result<FiveDaysForecastResult> _forecastWeather;

        /// <summary>
        /// Event fired when the weather has been updated.
        /// </summary>
        public EventHandler<UIWeatherUpdate> OnWeatherUpdate;

        /// <summary>
        /// Event fired when the forecasted weather has been updated.
        /// </summary>
        public EventHandler<UIWeatherUpdate[]> OnForecastUpdate;

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
        /// <param name="currentWeatherInterval">The time interval until the next update.</param>
        /// <param name="forecastUpdateInterval">The time interval until the next forecasted weather update.</param>
        public void SetUpdateInterval(TimeSpan? currentWeatherInterval = null, TimeSpan? forecastUpdateInterval = null)
        {
            if (currentWeatherInterval.HasValue && currentWeatherInterval.Value.TotalMilliseconds > 0)
            {
                if (_currentWeatherTimer == null)
                {
                    _currentWeatherTimer = new Timer(currentWeatherInterval.Value.TotalMilliseconds);
                    _currentWeatherTimer.AutoReset = true;
                    _currentWeatherTimer.Enabled = true;
                    _currentWeatherTimer.Elapsed += (object sender, ElapsedEventArgs e) => UpdateWeather();
                }

                _currentWeatherTimer.Stop();
                _currentWeatherTimer.Interval = currentWeatherInterval.Value.TotalMilliseconds;
                _currentWeatherTimer.Start();
            }
            else
            {
                _currentWeatherTimer.Stop();
                _currentWeatherTimer?.Dispose();
                _currentWeatherTimer = null;
            }

            if (forecastUpdateInterval.HasValue && forecastUpdateInterval.Value.TotalMilliseconds > 0)
            {
                if (_forecastWeatherTimer == null)
                {
                    _forecastWeatherTimer = new Timer(forecastUpdateInterval.Value.TotalMilliseconds);
                    _forecastWeatherTimer.AutoReset = true;
                    _forecastWeatherTimer.Enabled = true;
                    _forecastWeatherTimer.Elapsed += (object sender, ElapsedEventArgs e) => UpdateForecast();
                }

                _forecastWeatherTimer.Stop();
                _forecastWeatherTimer.Interval = forecastUpdateInterval.Value.TotalMilliseconds;
                _forecastWeatherTimer.Start();
            }
            else
            {
                _forecastWeatherTimer.Stop();
                _forecastWeatherTimer?.Dispose();
                _forecastWeatherTimer = null;
            }
        }

        /// <summary>
        /// Update the weather held in memory and broadcast the updates.
        /// </summary>
        /// <returns>Was the update successful</returns>
        public bool UpdateWeather()
        {
            try
            {
                SingleResult<CurrentWeatherResult> result;
                if (UseRealWeather)
                {
                    result = CurrentWeather.GetByCoordinates(SecretKeys.Latitude, SecretKeys.Longitude, "en", "imperial");
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
                    _currentWeather = result;
                    result.Item.Date = TimeZone.CurrentTimeZone.ToLocalTime(result.Item.Date);
                }
                else
                {
                    LastUpdateFailed = true;
                    Console.WriteLine($"Weather update failed with message: {result.Message}");
                }

                UIWeatherUpdate forUi = new UIWeatherUpdate(result.Item, !LastUpdateFailed);

                OnWeatherUpdate?.Invoke(this, forUi);
                return result.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception handled during weather update: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Update the forecasted weather held in memory and broadcast the updates.
        /// </summary>
        /// <returns>Was the update successful</returns>
        public bool UpdateForecast()
        {
            try
            {
                Result<FiveDaysForecastResult> result;
                if (UseRealWeather)
                {
                    result = FiveDaysForecast.GetByCoordinates(SecretKeys.Latitude, SecretKeys.Longitude, "en", "imperial");
                }
                else
                {
                    string debugWeatherFilePath = Path.Combine(Environment.CurrentDirectory, "fakeForecast.json");
                    string fakeWeatherJson = File.ReadAllText(debugWeatherFilePath);
                    result = JsonConvert.DeserializeObject<Result<FiveDaysForecastResult>>(fakeWeatherJson);
                    Console.WriteLine("Faked weather results!");
                }

                if (result.Success)
                {
                    LastUpdate = DateTimeOffset.Now;
                    LastUpdateFailed = false;
                    _forecastWeather = result;
                    result.Items.ForEach(r => r.Date = TimeZone.CurrentTimeZone.ToLocalTime(r.Date));
                }
                else
                {
                    LastUpdateFailed = true;
                    Console.WriteLine($"Weather update failed with message: {result.Message}");
                }

                // Determine the most severe weather during the next few days and choose those values to show.
                Dictionary<DateTime, FiveDaysForecastResult> highestRank = result.Items
                    .GroupBy(grp => grp.Date.Date)
                    .ToDictionary(grp => grp.Key, grp => HighestRankingWeatherUpdate(grp));

                UIWeatherUpdate[] forUi = highestRank.Values
                    .Select(f => new UIWeatherUpdate(f, !LastUpdateFailed))
                    .ToArray();
                OnForecastUpdate?.Invoke(this, forUi);

                return result.Success;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception handled during forecast update: {ex.Message}");
                return false;
            }
        }

        private FiveDaysForecastResult HighestRankingWeatherUpdate(IGrouping<DateTime, FiveDaysForecastResult> group)
        {
            Dictionary<string, int> weatherRanking = new Dictionary<string, int>()
            {
                { "Clear",  0 },
                { "Clouds",  1 },
                { "Drizzle",  2 },
                { "Rain",  3 },
                { "Thunderstorm",  4 },
                { "Snow",  5 },
                { "Atmosphere",  6 },
            };

            try
            {
                // "Take the first weather result that is not clear or cloudy or after nine in a list ordered by the severity and time."
                FiveDaysForecastResult highestResult = group.OrderByDescending(f => weatherRanking[f.Title])
                    .ThenBy(f => f.Date)
                    .First(f => weatherRanking[f.Title] >= 2 || f.Date.Hour >= 9);

                // Hacky but set the min/max as the value returned.
                highestResult.TempMax = group.Max(f => f.TempMax);
                highestResult.TempMin = group.Min(f => f.TempMin);

                return highestResult;
            }
            catch (KeyNotFoundException)
            {
                string firstUnknown = group.FirstOrDefault(g => !weatherRanking.ContainsKey(g.Title)).Title;
                Console.WriteLine($"Unknown weather title {firstUnknown}!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception occurred ranking forecasts. {e.Message}");
            }

            return group.FirstOrDefault(g => g.Date.Hour > 9);
        }
    }
}
