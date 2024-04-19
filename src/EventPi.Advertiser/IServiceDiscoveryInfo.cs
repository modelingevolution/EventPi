namespace EventPi.Advertiser;

public record ServiceAddresses : IEnumerable<Uri> 
{
    public  HostName Hostname { get; init; }
    public string ServiceName { get; init; }
    public IDictionary<InterfaceType, Uri> Urls { get; }
    
}

public enum InterfaceType
{
    Ethernet, Wifi
}