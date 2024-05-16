namespace EventPi.Pid;

public static class Extensions
{
    public static PidControllerTimeWrapper<TPid> SelfTimed<TPid>(this TPid p)
        where TPid : IPidConfig, IController
    {
        return new PidControllerTimeWrapper<TPid>(p);
    }
}