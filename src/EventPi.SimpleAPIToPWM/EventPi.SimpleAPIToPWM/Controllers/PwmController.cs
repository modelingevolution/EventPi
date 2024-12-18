using Microsoft.AspNetCore.Mvc;
using System.Device.Gpio;
using System.Device.Pwm;

namespace EventPi.SimpleAPIToPWM.Controllers
{
    public class PinsController
    {
        private readonly PwmChannel _motor;
        private readonly GpioController _controller;
        private readonly GpioPin _dirPin;
        public int IsRunning;
        public PinsController()
        {
            _motor = PwmChannel.Create(2, 0, 37000, 0.5);
            _controller= new GpioController(PinNumberingScheme.Logical);
            _dirPin=_controller.OpenPin(26, PinMode.Output);
        }
        public void Start()
        { 
            _motor.Start(); 
        }
        public void Stop()
        {
            _motor.Stop();
        }
        public void ChangeToForward()
        {
            Console.WriteLine($"Pin 26 high");
            _dirPin.Write(PinValue.High);
        }
        public void ChangeToBackward()
        {
            Console.WriteLine($"Pin 26 low");
            _dirPin.Write(PinValue.Low);
        }
    }
    [ApiController]
    [Route("[controller]")]
    public class PwmController: ControllerBase
    {
      

        private readonly ILogger<PwmController> _logger;
        private readonly PinsController _ctr;

        public PwmController(ILogger<PwmController> logger, PinsController ctr)
        {
            _logger = logger;
            
            //var freq = Int32.Parse(Console.ReadLine());
            _ctr=ctr;

        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task RunMotor(int duration)
        {
            Console.WriteLine($"Engine started, duration:{duration}");
            Interlocked.Increment(ref _ctr.IsRunning);
            if (duration>0)
            {

                _ctr.ChangeToForward();
            }
            else
            {
                
                duration=(-1)*duration;
                _ctr.ChangeToBackward();
            }
            _ctr.Start();
            Thread.Sleep(duration);
            _ctr.Stop();
            Interlocked.Decrement(ref _ctr.IsRunning);
            Console.WriteLine($"Engine stopped");
        }
        [HttpPost("{duration}")]
        public void Post(int duration)
        {
            if (_ctr.IsRunning==0)
                Task.Run(()=>RunMotor(duration));
        }
    }
}
