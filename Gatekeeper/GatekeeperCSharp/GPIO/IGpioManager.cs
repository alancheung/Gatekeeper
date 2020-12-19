using System;
using System.Collections.Generic;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.RaspberryIO.Peripherals;

namespace GatekeeperCSharp.GPIO
{
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
        void Toggle(BcmPin pin, GpioPinValue initial, TimeSpan duration);
    }
}
