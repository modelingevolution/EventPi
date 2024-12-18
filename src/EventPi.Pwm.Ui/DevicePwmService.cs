using EventPi.Pid;
using System.Device.Pwm;

namespace EventPi.Pwm.Ui
{
    public class DevicePwmService : IPwmService
    {
        readonly PwmChannel _channel;
        public bool IsReverse { get; set; }
        public bool IsRunning { get; private set; }
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
