using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.RaspberryIO.Peripherals;
using Unosquare.WiringPi;
using static Unosquare.RaspberryIO.Peripherals.RFIDControllerMfrc522;

namespace GatekeeperCSharp.GPIO
{
    public class GpioManager : IGpioManager, IDisposable
    {
        private Thread RfidReadThread { get; set; }
        private CancellationTokenSource RfidReadThreadCancellationSource { get; set; }

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
            Pi.Init<BootstrapWiringPi>();

            foreach (KeyValuePair<BcmPin, GpioPinDriveMode> config in pins)
            {
                Pi.Gpio[config.Key].PinMode = config.Value;
            }

            Rfid = new RFIDControllerMfrc522();

            RfidReadThreadCancellationSource = new CancellationTokenSource();
            RfidReadThread = new Thread(RfidReadAction);
            RfidReadThread.Start();
        }

        public void SetPin(BcmPin pin, GpioPinValue value)
        {
            Pi.Gpio[pin].Write(value);
        }

        public void Toggle(BcmPin pin, GpioPinValue initial, TimeSpan duration)
        {
            GpioPinValue next = initial == GpioPinValue.High ? GpioPinValue.Low : GpioPinValue.High;

            SetPin(pin, initial);
            Thread.Sleep(duration);
            SetPin(pin, next);
        }

        /// <summary>
        /// Threaded action to listen for RFID events.
        /// </summary>
        public void RfidReadAction()
        {
            while (!RfidReadThreadCancellationSource.IsCancellationRequested)
            {
                if (Rfid.DetectCard() == Status.AllOk)
                {
                    RfidResponse card = Rfid.ReadCardUniqueId();

                    if (card.Status == Status.AllOk)
                    {
                        OnRfidCardDetected?.Invoke(this, new RfidDetectedEventArgs() { Data = card.Data });
                    }
                    else
                    {
                        Console.WriteLine($"Card detected but failed to read ({card.Status})");
                    }
                }
            }
        }

        /// <summary>
        /// Implementation of <see cref="IDisposable"/>
        /// </summary>
        public void Dispose()
        {
            RfidReadThreadCancellationSource?.Cancel();
            RfidReadThread?.Abort();

            Console.WriteLine($"{nameof(GpioManager)} disposing; " +
                $"{nameof(RfidReadThreadCancellationSource)}({RfidReadThreadCancellationSource?.IsCancellationRequested}); " +
                $"{nameof(RfidReadThread)}({RfidReadThread?.ThreadState})");
        }
    }
}
