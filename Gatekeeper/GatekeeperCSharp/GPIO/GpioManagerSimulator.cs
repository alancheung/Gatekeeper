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
        public DhtSensor Dht22 { get; set; }

        public event EventHandler<RfidDetectedEventArgs> OnRfidCardDetected;
        public event EventHandler<DhtEventArgs> OnValidDhtData;

        private static Random randy = new Random();
        private System.Timers.Timer _dhtDataTimer;

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
            Dht22 = null;
            _dhtDataTimer = new System.Timers.Timer(TimeSpan.FromSeconds(2).TotalMilliseconds);
            _dhtDataTimer.Elapsed += _dhtDataTimer_Elapsed;
            _dhtDataTimer.AutoReset = true;
            _dhtDataTimer.Enabled = true;
        }

        private void _dhtDataTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            InvokeOnValidDhtData();
        }

        public void SetPin(BcmPin pin, GpioPinValue value)
        {
            state[pin].Add(value);
            Console.WriteLine($"Set pin {pin} to {value}.");
        }

        public async Task Toggle(BcmPin pin, GpioPinValue initial, TimeSpan duration)
        {
            await Task.Run(() => ToggleAction(pin, initial, duration));
        }

        private void ToggleAction(BcmPin pin, GpioPinValue initial, TimeSpan duration)
        {
            GpioPinValue next = initial == GpioPinValue.High ? GpioPinValue.Low : GpioPinValue.High;
            SetPin(pin, initial);
            Console.WriteLine($"Sleeping for {duration.TotalMilliseconds} milliseconds.");
            Thread.Sleep(duration);
            SetPin(pin, next);
            Console.WriteLine("Done with sleep!");
        }

        public void InvokeOnRfidCardDetected()
        {
            OnRfidCardDetected?.Invoke(this, new RfidDetectedEventArgs()
            {
                Data = new byte[4] { 1, 2, 3, 4 }
            });
        }

        public void InvokeOnValidDhtData()
        {
            OnValidDhtData?.Invoke(null, new DhtEventArgs() { Humidity = randy.NextDouble(), Temperature = randy.NextDouble() });
        }
    }
}
