using GatekeeperCSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.RaspberryIO.Peripherals;

namespace GatekeeperCSharp.GPIO
{
    public class GpioManagerSimulator : IGpioManager
    {
        Dictionary<BcmPin, List<GpioPinValue>> state;

        public RFIDControllerMfrc522 Rfid { get; set; }

        public bool AddingNewRfidCard { get; set; } = false;

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
            Task unlockTask = Task.Run(() => ToggleAction(pin, initial, duration));
            unlockTask.Wait();
        }

        private void ToggleAction(BcmPin pin, GpioPinValue initial, TimeSpan duration)
        {
            GpioPinValue next = initial == GpioPinValue.High ? GpioPinValue.Low : GpioPinValue.High;
            SetPin(pin, initial);
            Console.WriteLine($"Sleeping for {duration.TotalMilliseconds} milliseconds.");
            Thread.Sleep(duration);
            SetPin(pin, next);
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
