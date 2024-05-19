namespace EventPi;

public record ServiceInfo 
{
    public ServiceProperty[] Properties { get; } = [];
    public string Schema { get; init; }
    public string ServiceName { get; init; }
    public int? Port { get; init; }
    public ServiceInfo(string ServiceName, int? Port) : this("tcp", ServiceName, Port) { }

    public ServiceInfo(string Schema, string ServiceName, int? Port, params ServiceProperty[] properties)
    {
        this.Schema = Schema;
        this.ServiceName = ServiceName;
        this.Port = Port;
        this.Properties = properties;
    }

    public void Deconstruct(out string Schema, out string ServiceName, out int? Port)
    {
        Schema = this.Schema;
        ServiceName = this.ServiceName;
        Port = this.Port;
    }
}
public record ServiceProperty(string Key, string Value);