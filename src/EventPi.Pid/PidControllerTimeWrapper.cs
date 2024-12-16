using System.Diagnostics;

namespace EventPi.Pid;

public class PidControllerTimeWrapper<TPid>(TPid _pid) : IPidConfig, IController
    where TPid : IPidConfig, IController
{
    private readonly Stopwatch _sw = Stopwatch.StartNew();
    public void Start() => _sw.Restart();
    public double Kp
    {
        get => _pid.Kp;
        set => _pid.Kp = value;
    }

    public double Ki
    {
        get => _pid.Ki;
        set => _pid.Ki = value;
    }

    public double Kd
    {
        get => _pid.Kd;
        set => _pid.Kd = value;
    }

    public double OutputUpperLimit
    {
        get => _pid.OutputUpperLimit;
        set => _pid.OutputUpperLimit = value;
    }

    public double OutputLowerLimit
    {
        get => _pid.OutputLowerLimit;
        set => _pid.OutputLowerLimit = value;
    }
    public double Compute(double setPoint, double processValue)
    {
        var ts = _sw.Elapsed;
        
        var r = _pid.Compute(setPoint, processValue, ts);
        _sw.Restart();
        return r;
    }

    public double Compute(double setPoint, double processValue, long milliseconds)
    {
        var ts = TimeSpan.FromMilliseconds(milliseconds);
        return _pid.Compute(setPoint, processValue, ts);
    }
    public double Compute(double setPoint, double processValue, TimeSpan ts)
    {
        return _pid.Compute(setPoint, processValue, ts);
    }
}