using System.Threading;

namespace EventPi.Pid;

public class StepMotorModel
{
    private double _lastPosition = 0;
    private DateTime _started;
    private DateTime _finish;
    private int _targetSteps;
    private double _target = 0;
    private double _stepDeg;
    private double _frequency;
    private double _dt;
    private double _rpm;
    private double _distancePerRotation;
    private double _precision;
    
  
    public double StepDeg => _stepDeg;
    public double Frequency => _frequency;
    public double Rpm => _rpm;
    public double Precision => _precision;
    public double Velocity => _precision / _dt;
    
    public StepMotorModel(double stepDeg, double frequency, double distancePerRotation, 
        double initPosition = 0d)
    {
        _lastPosition = initPosition;
        _stepDeg = stepDeg;
        _frequency = frequency;
        _dt = 1 / _frequency;
        _distancePerRotation = distancePerRotation;

        // Calculate maximum RPM based on step degree and frequency
        _rpm = frequency / (360 / stepDeg) * 60;
        _targetSteps = 0;
        // Calculate precision in mm
        _precision = distancePerRotation / (360 / stepDeg);
    }

    public void Calibrate(double currentPosition)
    {
        _lastPosition = currentPosition;
    }
    public int CalculateSteps(double start, double end)
    {
        return CalculateSteps(end - start);
    }
    public int CalculateSteps(double distance)
    {
        var ticks = (int)Math.Round(distance / _precision);
        return ticks;
    }

    public TimeSpan ComputeDuration(int steps) => TimeSpan.FromSeconds(Math.Abs(steps) * _dt);

    public bool IsRunning() => IsRunning(DateTime.Now);
    public bool IsRunning(DateTime n) => _started <= n && n <= _finish;

    public double Position(DateTime? when = null)
    {
        var n = when ?? DateTime.Now;
        if (!IsRunning(n))
        {
            if(_started == DateTime.MinValue)
                return _lastPosition;
            _lastPosition = _target;
            _started = _finish = DateTime.MinValue;
            return _lastPosition;
        }

        var elapsedTime = n - _started;
        return Position(elapsedTime);
    }

    public double Rotations(TimeSpan dt) => _frequency * dt.TotalSeconds;
    public double Position(TimeSpan dt) => _lastPosition + Distance(dt);

    public double Distance(TimeSpan dt)
    {
        var elapsedSteps = (int)(dt.TotalSeconds / _dt);
        var distance = elapsedSteps * _precision;
        return distance;
    }

    public MoveDirection LastDirection => _targetSteps > 0 ? MoveDirection.Forward : MoveDirection.Backward;
    public double Target
    {
        get => _target;
    }

    public enum MotorAction
    {
        Continue, Start, Reverse, Pause
    }

    public enum MoveDirection
    {
        Forward,
        Backward
    }

    public DateTime MoveTo(double targetPos) => MoveTo(targetPos, out var _);
    /// <summary>
    /// When invoked it means that from now we need to move by distance.
    /// It means that we might, change direction of the motor.
    /// </summary>
    /// <param name="targetPos">The distance.</param>
    public DateTime MoveTo(double targetPos, out MotorAction action, double? currentPosition = null)
    {
        // If the motor is already moving, we check direction and adjust accordingly
        var n = DateTime.Now;
        if (IsRunning(n))
        {
            var currentPos = currentPosition ?? Position(n);
            if ((targetPos >= currentPos && _target >= currentPos) || (targetPos <= currentPos && _target <= currentPos))
            {
                // Same direction: Update the target and finish time
                _target = targetPos;
                _targetSteps = CalculateSteps(currentPos, targetPos);
                var remainingSteps = CalculateSteps(currentPos, _target);
                _finish = n.Add(ComputeDuration(remainingSteps));
                action = MotorAction.Continue;
                if (currentPosition != null)
                {
                    _lastPosition = currentPosition.Value;
                }
                
            }
            else
            {
                _lastPosition = currentPos;
                _started = n; // Adjust start time
                _target = targetPos;
                _targetSteps = CalculateSteps(currentPos, targetPos);
                _finish = _started.Add(ComputeDuration(_targetSteps));
                action = MotorAction.Reverse;
            }
        }
        else
        {
            // If motor is not moving, simply start the movement
            if (currentPosition != null) _lastPosition = currentPosition.Value;
            if (Math.Abs(_lastPosition - targetPos) < double.E)
            {
                action = MotorAction.Pause;
                return n;
            }
            _started = n;
            _target = targetPos;
            _targetSteps = CalculateSteps(_lastPosition, targetPos);
            _finish = _started.Add(ComputeDuration(_targetSteps)); // Ile Pwm ma smigac, podpinamy sie eventhandlerem ze motor powinnien sie ruszyc o timespan
            action = MotorAction.Start;

        }
        return _finish;
    }

    public override string ToString()
    {
        // add new line between values
        return string.Join(Environment.NewLine,
            new string[]
            {
                $"Step (deg): {_stepDeg}", 
                $"Frequency (Hz): {_frequency}", 
                $"Rpm: {_rpm}", 
                $"Precision (mm): {_precision}",
                $"Velocity (mm/s): {Velocity}"
            });
        
    }
}