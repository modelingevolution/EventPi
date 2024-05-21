using System.Diagnostics;

namespace EventPi.Pid;

public class PidControllerTimeWrapper<TPid>(TPid _pid) : IPidConfig, IController
    where TPid : IPidConfig, IController
{
    private Stopwatch _sw = new Stopwatch();
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
    public double CalculateOutput(double setPoint, double processValue)
    {
        var ts = _sw.Elapsed;
        var r = _pid.CalculateOutput(setPoint, processValue, ts);
        _sw.Restart();
        return r;
    }
    public double CalculateOutput(double setPoint, double processValue, TimeSpan ts)
    {
        return _pid.CalculateOutput(setPoint, processValue, ts);
    }
}