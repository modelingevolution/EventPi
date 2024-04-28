namespace EventPi;

public record ServiceInfo : IServiceInfo
{
    public ServiceInfo(string ServiceName, int Port) : this("tcp", ServiceName, Port) { }
    public ServiceInfo(string Schema, string ServiceName, int Port)
    {
        this.Schema = Schema;
        this.ServiceName = ServiceName;
        this.Port = Port;
    }

    public string Schema { get; init; }
    public string ServiceName { get; init; }
    public int Port { get; init; }

    
}