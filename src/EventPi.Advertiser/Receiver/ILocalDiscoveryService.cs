namespace EventPi.Advertiser.Receiver;

public interface ILocalDiscoveryService
{
    public IEnumerable<ServiceAddress> GetService(ServiceName serviceName);
    event EventHandler<ServerDiscoveredEventArgs> ServiceFound;
    event EventHandler<ServerDiscoveredEventArgs> ServiceLost;
}