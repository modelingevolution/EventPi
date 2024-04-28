namespace EventPi.Advertiser.Receiver;

public interface IServiceName { }
public readonly record struct ServiceName(string name) : IServiceName
{
    public override string ToString() => name;
    public static implicit operator ServiceName(string name) => new ServiceName(name);
}