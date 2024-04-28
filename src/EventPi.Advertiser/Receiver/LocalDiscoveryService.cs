using System.Collections.Concurrent;
using System.Text.Json;
using Zeroconf;

namespace EventPi.Advertiser.Receiver;

//public class DiscoveryServiceLiteDbPersister
//{

//    public DiscoveryServiceLiteDbPersister(ILocalDiscoveryService discovery)
//    {
//        discovery.Se
//    }

//    private void RegisterService()
//    {


//    }
//}

public class LocalDiscoveryService : ILocalDiscoveryService
{

    private readonly ConcurrentDictionary<ServiceName,ZeroconfResolver.ResolverListener> _listeners;
    private readonly ConcurrentDictionary<ServiceName, ServiceAddresses?> _servicesAddresses;
    private readonly ConcurrentDictionary<ServiceName, ServerDiscoveredEventArgs> _dictOfEvents;

    private readonly object _sync = new object();

    private EventHandler<ServerDiscoveredEventArgs> _serviceFound;
    public event EventHandler<ServerDiscoveredEventArgs> ServiceFound
    {
        add
        {
            lock (_sync)
            {

                foreach (var ev in _dictOfEvents.Values)
                {
                    value(this, ev);
                }

                _serviceFound += value;
            }
        }
        remove
        {
            lock (_sync)
            {
                _serviceFound -= value;
            }
        }
    }
    public event EventHandler<ServerDiscoveredEventArgs> ServiceLost;

    public LocalDiscoveryService(IEnumerable<IServiceName> servicesToRegister)
    {
        _listeners = new ConcurrentDictionary<ServiceName, ZeroconfResolver.ResolverListener>();
        _servicesAddresses = new ConcurrentDictionary<ServiceName, ServiceAddresses?>();
        _dictOfEvents = new ConcurrentDictionary<ServiceName, ServerDiscoveredEventArgs>();

        foreach (var service in servicesToRegister) RegisterListener((ServiceName)service);
    }

    public ServiceAddresses? GetService(ServiceName serviceName) => _servicesAddresses.GetValueOrDefault(serviceName);

    private void RegisterListener(ServiceName serviceName)
    {
        var listener = ZeroconfResolver.CreateListener(serviceName.ToString(), 2000);
        listener.ServiceFound += AddService;
        _listeners.TryAdd(serviceName, listener);
    }

    private void AddService(object sender, IZeroconfHost host)
    {
        var srvs = host.Services;
        var properties = srvs.Values.First().Properties;
        RpiAdvertiseTools.GetWifiAndEthernet(properties, out string wifiAddress, out string ethernetAddress);
        var dict = new ConcurrentDictionary<InterfaceType, Uri>();

        if(ethernetAddress!=String.Empty)
             dict.TryAdd(InterfaceType.Ethernet, new Uri(ethernetAddress));
        if (wifiAddress!=String.Empty)
            dict.TryAdd(InterfaceType.Wifi, new Uri(wifiAddress));

        _servicesAddresses.TryAdd(srvs.Values.First().ServiceName, new ServiceAddresses()
        {
            Hostname = (HostName)host.DisplayName,
            ServiceName = srvs.Values.First().ServiceName,
            Urls = dict
        });

        var ev = new ServerDiscoveredEventArgs()
        {
            Hostname = (HostName)host.DisplayName,
            ServiceName = srvs.Values.First().ServiceName,
            Urls = dict

        };
        lock (_sync)
        {
            _serviceFound.Invoke(this, ev);
            _dictOfEvents.TryAdd(srvs.Values.First().ServiceName, ev);
        }

    }
}

