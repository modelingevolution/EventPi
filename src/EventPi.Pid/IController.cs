namespace EventPi.Pid;

public interface IController
{
    double Compute(double setPoint, double processValue, TimeSpan ts);
}