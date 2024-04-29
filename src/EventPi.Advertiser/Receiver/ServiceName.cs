namespace EventPi.Advertiser.Receiver;

public interface IServiceName { }

public readonly record struct ServiceInstance(ServiceName Name, HostName Host);

public record ServiceName : IServiceName
{
    private readonly string _name;
    private ServiceName(string name) => this._name = name;

    public override string ToString() => _name;
    public static implicit operator ServiceName(string name) => new ServiceName(name);
    
}