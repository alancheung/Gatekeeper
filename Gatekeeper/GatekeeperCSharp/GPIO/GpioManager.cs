﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace GatekeeperCSharp.GPIO
{
    public class GpioManager : IGpioManager
    {
        public void Initialize(BcmPin pin, GpioPinDriveMode mode)
        {
            Pi.Gpio[pin].PinMode = mode;
        }

        public void Initialize(Dictionary<BcmPin, GpioPinDriveMode> pins)
        {
            Pi.Init<BootstrapWiringPi>();

            foreach (KeyValuePair<BcmPin, GpioPinDriveMode> config in pins)
            {
                Pi.Gpio[config.Key].PinMode = config.Value;
            }
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
    }
}
