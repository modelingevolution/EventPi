using EventPi.Pid;
using Microsoft.AspNetCore.Mvc;
using System.Device.Gpio;
using System.Device.Pwm;

namespace EventPi.Pwm.Ui
{
    public class DevicePwmService : IPwmService
    {
        private readonly PwmChannel _channel;
        private readonly GpioController _controller;
        private readonly GpioPin _dirPin;

        private bool _isReverse;
        public bool IsReverse
        {
            get => _isReverse;
            set
            {
                if (_isReverse== value) return;
                _isReverse= value;
                _dirPin.Write(value == true ? PinValue.Low : PinValue.High);
            }
        }
        public bool IsRunning { get; private set; }

        public DevicePwmService()
        {
            _channel=PwmChannel.Create(2, 0, 37000, 0.5);
            _controller= new GpioController(PinNumberingScheme.Logical);
            _dirPin=_controller.OpenPin(26, PinMode.Output);
           
        }
     

        public void Start()
        {
            _channel.Start();
            IsRunning = true;
        }

        public void Stop()
        {
            _channel.Stop();
            IsRunning = false;
        }

        public int Frequency
        {
            get => _channel.Frequency;
            set => _channel.Frequency = value;
        }

        public double DutyCycle
        {
            get => _channel.DutyCycle;
            set => _channel.DutyCycle = value;
        }
    }
}
