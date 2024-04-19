namespace EventPi.Advertiser;

public  class ServiceDiscoveryInfo : IServiceDiscoveryInfo
{
    public HostName Hostname { get; init; }
    public string IPAddressWlan0 { get; set; }
    public string IPAddressEth0 { get; set; }
    public int Port { get; init; }
}