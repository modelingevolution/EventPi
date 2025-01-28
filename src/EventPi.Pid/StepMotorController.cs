using System.Data;
using System.Threading.Channels;
using EventPi.SignalProcessing;
using Microsoft.Extensions.Logging;

namespace EventPi.Pid;
public class PidService : PidControllerTimeWrapper<PidController>
{

    public PidService(double kp, double kd, double ki, double ou, double ol, double? ig) : base(new PidController(kp, kd, ki, ou, ol, ig))
    {
    }

}
public class StepMotorController : IDisposable
{
    private readonly IPwmService _pwm;
    private readonly PidService _pid;
    private readonly ILogger<StepMotorController> _logger;
    private readonly StepMotorModel _model;
    private readonly ManualResetEventSlim _manualResetEvent;
    private readonly CancellationTokenSource _cts;
    public StepMotorModel MotorModel => _model;
    public double Target { get; private set; }
    public double ProcessedValue { get; private set; }
    public StepMotorController(IPwmService pwm, PidService pid,
        ILogger<StepMotorController> logger)
    {
        _cts = new();
        _pwm =pwm;
        _pid = pid;
        _logger = logger;
        _model = new StepMotorModel(1.8, 38000, 0.5, 0);
        _manualResetEvent = new ManualResetEventSlim(false);
        _ = Task.Factory.StartNew(OnDispatch, TaskCreationOptions.LongRunning);

    }

    public void MoveTo(double targetValue, double currentValue)
    {
        _commands.Writer.TryWrite(new Command(targetValue, currentValue));
        _manualResetEvent.Set();
    }

    private readonly record struct MotorDelayedAction(
        DateTime ValidUntil,
        StepMotorModel.MotorAction Action,
        StepMotorModel.MoveDirection Direction)
    {
        public override string ToString()
        {
            // make sure to include miliseconds
            var f = ValidUntil.Subtract(DateTime.Now);
            return $"Valid until: {ValidUntil:HH:mm:ss.fff}, Valid for: {f.TotalMilliseconds}, Action: {Action}, Direction: {Direction}";
        }
    }

    private readonly record struct Command(double Target, double ProcessedValue);
    private readonly Channel<Command> _commands = Channel.CreateBounded<Command>(new BoundedChannelOptions(1)
    {
        Capacity = 1,
        FullMode = BoundedChannelFullMode.DropOldest, 
        SingleReader = true, 
        SingleWriter = false
    });
    
    private async Task OnDispatch()
    {
        try
        {
            _pid.Start();
            MotorDelayedAction? delayedAction = null;
            while (!_cts.IsCancellationRequested)
            {
                if (delayedAction.HasValue)
                {
                    _logger.LogInformation("Delayed action: {action}", delayedAction.Value);
                    var waitUntil = delayedAction.Value.ValidUntil;
                    var timeToWait = waitUntil.Subtract(DateTime.Now);
                    if (timeToWait.Ticks > 0)
                    {
                        var timeSpan = timeToWait.Subtract(TimeSpan.FromMilliseconds(50));
                        
                        var wokenManually = timeSpan.Ticks > 0 && _manualResetEvent.Wait(timeSpan);
                        if (!wokenManually)
                        {
                            // it means we have time-out. Let's check if we don't need to spin
                            while (waitUntil.Subtract(DateTime.Now).TotalMilliseconds > 0)
                                Thread.SpinWait(200);

                            _pwm.Stop();
                            delayedAction = null;
                        }
                        else
                        {
                            _manualResetEvent.Reset();
                            // it means that a command was send.
                            if (_commands.Reader.TryRead(out var cmd))
                            {
                                Update(cmd);
                                var adjustment = _pid.Compute(cmd.Target, cmd.ProcessedValue);
                                
                                var nextValidUntil = _model.MoveTo(cmd.Target + adjustment, out var action, cmd.ProcessedValue);
                                if (action == StepMotorModel.MotorAction.Pause) continue;
                                
                                _pwm.IsReverse = _model.LastDirection == StepMotorModel.MoveDirection.Backward;
                                delayedAction = new MotorDelayedAction(nextValidUntil, action, _model.LastDirection);
                            } // We don't care about else, it means we already processed it.
                        }
                    }
                    else
                    {
                        // it means we've waited too long
                        _pwm.Stop();
                        delayedAction = null;
                    }
                }
                else
                {
                    _logger.LogInformation("Waiting for commands.");
                    var cmd = await _commands.Reader.ReadAsync(_cts.Token);
                    Update(cmd);
                    var adjustment = _pid.Compute(cmd.Target, cmd.ProcessedValue);
                    var until = _model.MoveTo(cmd.Target + adjustment, out var action, cmd.ProcessedValue);
                    if(action == StepMotorModel.MotorAction.Pause) continue;
                    
                    delayedAction = new MotorDelayedAction(until, action, _model.LastDirection);
                    _pwm.IsReverse = _model.LastDirection == StepMotorModel.MoveDirection.Backward;
                    _pwm.Start();
                }



            }
        }
        catch (OperationCanceledException)
        {

        }
        finally
        {
            _pwm.Stop();
        }
    }

    private Command Update(Command cmd)
    {
        this.Target = cmd.Target;
        this.ProcessedValue = cmd.ProcessedValue;
        return cmd;
    }

    private bool _disposed = false;
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _cts.Cancel();
        _cts.Dispose();
        _manualResetEvent.Dispose();
        
    }
}