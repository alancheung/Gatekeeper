using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.RaspberryIO.Peripherals;

namespace GatekeeperCSharp.GPIO
{
    /// <summary>
    /// <see cref="EventArgs"/> thrown when a RFID card is detected.
    /// </summary>
    public class RfidDetectedEventArgs : EventArgs
    {
        /// <summary>
        /// Data read from the RFID card.
        /// </summary>
        public byte[] Data { get; set; }
    }

    public class DhtEventArgs : EventArgs
    {
        public double Temperature;
        public double Humidity;
    }

    /// <summary>
    /// Application level wrapper around GPIO functions allowing simulation of GPIO elements.
    /// </summary>
    public interface IGpioManager
    {
        /// <summary>
        /// RFID controller
        /// </summary>
        RFIDControllerMfrc522 Rfid { get; set; }

        /// <summary>
        /// Is the user currently saving a new card?
        /// </summary>
        bool AddingNewRfidCard { get; set; }

        /// <summary>
        /// Event fired when an RFID is read
        /// </summary>
        event EventHandler<RfidDetectedEventArgs> OnRfidCardDetected;

        /// <summary>
        /// DHT22 sensor
        /// </summary>
        DhtSensor Dht22 { get; set; }

        /// <summary>
        /// Event fired when new Dht22 sensor data is available
        /// </summary>
        event EventHandler<DhtEventArgs> OnValidDhtData;

        /// <summary>
        /// Initialize <see cref="Pi.Gpio"/> wrapper with a single <paramref name="pin"/> to <paramref name="mode"/>.
        /// </summary>
        void Initialize(BcmPin pin, GpioPinDriveMode mode);

        /// <summary>
        /// Initialize <see cref="Pi.Gpio"/> wrapper and set all <see cref="GpioPinDriveMode"/> on pins.
        /// </summary>
        void Initialize(Dictionary<BcmPin, GpioPinDriveMode> pins);

        /// <summary>
        /// Set the <paramref name="pin"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="pin">Pin to set</param>
        /// <param name="value">Value to set to</param>
        void SetPin(BcmPin pin, GpioPinValue value);

        ///// <summary>
        ///// Toggle the <paramref name="pin"/> beginning with <paramref name="initial"/>.
        ///// </summary>
        ///// <param name="pin">Pin to set</param>
        ///// <param name="value">Value to set to</param>
        //void Toggle(BcmPin pin, GpioPinValue initial);

        /// <summary>
        /// Toggle the <paramref name="pin"/> for <paramref name="duration"/>.
        /// </summary>
        /// <param name="pin">Pin to set</param>
        /// <param name="value">Value to set to</param>
        /// <returns>Async toggle task</returns>
        Task Toggle(BcmPin pin, GpioPinValue initial, TimeSpan duration);
    }
}
