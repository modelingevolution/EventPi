namespace EventPi.Pid;

public interface IController
{
    double CalculateOutput(double setPoint, double processValue, TimeSpan ts);
}