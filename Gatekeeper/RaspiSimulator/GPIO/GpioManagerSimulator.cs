using GatekeeperCSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.RaspberryIO.Peripherals;

namespace RaspiSimulator.GPIO
{
    public class GpioManagerSimulator : IGpioManager
    {
        Dictionary<BcmPin, List<GpioPinValue>> state;

        public RFIDControllerMfrc522 Rfid { get; set; }

        public event EventHandler<RfidDetectedEventArgs> OnRfidCardDetected;

        public void Initialize(BcmPin pin, GpioPinDriveMode mode)
        {
            Initialize(new Dictionary<BcmPin, GpioPinDriveMode>()
            {
                { pin, mode }
            });
        }

        public void Initialize(Dictionary<BcmPin, GpioPinDriveMode> pins)
        {
            state = pins.ToDictionary(key => key.Key, val => new List<GpioPinValue>() { GpioPinValue.Low });
            Console.WriteLine($"Initialized pins: {state.Stringify(s => s.Key)}.");

            Rfid = null;
        }

        public void SetPin(BcmPin pin, GpioPinValue value)
        {
            state[pin].Add(value);
            Console.WriteLine($"Set pin {pin} to {value}.");
        }

        public void Toggle(BcmPin pin, GpioPinValue initial, TimeSpan duration)
        {
            GpioPinValue next = initial == GpioPinValue.High ? GpioPinValue.Low : GpioPinValue.High;
            state[pin].Add(initial);
            state[pin].Add(next);

            Console.WriteLine($"Set pin {pin} to {initial} then {next} after waiting for {duration.TotalMilliseconds} milliseconds.");
        }

        public void InvokeOnRfidCardDetected()
        {
            OnRfidCardDetected?.Invoke(this, new RfidDetectedEventArgs()
            {
                Data = new byte[4] { 1, 2, 3, 4 }
            });
        }
    }
}
