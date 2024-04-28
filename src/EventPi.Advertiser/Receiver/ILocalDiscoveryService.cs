namespace EventPi.Advertiser.Receiver;

public interface ILocalDiscoveryService
{
    public ServiceAddresses? GetService(ServiceName serviceName);
    event EventHandler<ServerDiscoveredEventArgs> ServiceFound;
    event EventHandler<ServerDiscoveredEventArgs> ServiceLost;
}