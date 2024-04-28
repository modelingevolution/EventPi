namespace EventPi.Advertiser.Receiver;

public interface IServiceName { }
public record ServiceName : IServiceName
{
    private readonly string _name;
    private ServiceName(string name) => this._name = name;

    public override string ToString() => _name;
    public static implicit operator ServiceName(string name) => new ServiceName(name);
    
}