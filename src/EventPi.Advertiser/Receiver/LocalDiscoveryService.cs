using System.Collections.Concurrent;
using System.Text.Json;
using Makaretu.Dns.Resolving;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<LocalDiscoveryService> _logger;

    private readonly ConcurrentDictionary<ServiceName, ZeroconfResolver.ResolverListener> _listeners = new();
    private readonly ConcurrentDictionary<ServiceName, ConcurrentSet<ServiceAddress>?> _servicesAddresses = new();
    private readonly ConcurrentSet<ServiceInstance> _serviceInstances = new();
    private readonly ConcurrentBag<ServerDiscoveredEventArgs> _services = new();
    private readonly object _sync = new object();

    private EventHandler<ServerDiscoveredEventArgs> _serviceFound;
    public event EventHandler<ServerDiscoveredEventArgs> ServiceFound
    {
        add
        {
            lock (_sync)
            {
                try
                {
                    foreach (var ev in _services) value(this, ev);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ServiceFound event thrown exception.");
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

    public LocalDiscoveryService(ILogger<LocalDiscoveryService> logger, IEnumerable<IServiceName> servicesToRegister)
    {
        _logger = logger;
        foreach (var service in servicesToRegister) RegisterListener((ServiceName)service);
    }

    public IEnumerable<ServiceAddress> GetService(ServiceName serviceName) => _servicesAddresses.GetOrAdd(serviceName, x => new ())!;

    private void RegisterListener(ServiceName serviceName)
    {
        var protocol = serviceName.ToString();
        if (!protocol.EndsWith(".local."))
            protocol += ".local.";
        var listener = ZeroconfResolver.CreateListener(protocol, 2000);
        listener.ServiceFound += AddService;
        _listeners.TryAdd(serviceName, listener);

        _logger.LogInformation($"Listening for service: {serviceName}.");
    }

    private void AddService(object sender, IZeroconfHost host)
    {
        var srvs = host.Services;
        foreach (var srv in srvs)
        {

            var properties = srv.Value.Properties;
            var port = srv.Value.Port;

            DiscoveryProperties.RetriveProperties(properties, out string wifiAddress, out string ethernetAddress, out var schema);

            string service = srv.Value.ServiceName;
            if (service.StartsWith($"{host.DisplayName}."))
                service = service.Substring(host.DisplayName.Length + 1);
            if (service.EndsWith(".local."))
                service = service.Remove(service.Length - 7);

            var hostName = (HostName)host.DisplayName;
            var serviceInstance = new ServiceInstance(service, hostName);

            if (!_serviceInstances.Add(serviceInstance))
                return;


            var urls = new Dictionary<InterfaceType, Uri>();
            if (ethernetAddress != String.Empty) 
                urls.TryAdd(InterfaceType.Ethernet, new Uri($"{schema}://{ethernetAddress}:{port}"));

            if (wifiAddress != String.Empty) 
                urls.TryAdd(InterfaceType.Wifi, new Uri($"{schema}://{wifiAddress}:{port}"));

            
            
            _servicesAddresses.GetOrAdd(service, x => new ConcurrentSet<ServiceAddress>())!
                .Add(new ServiceAddress()
            {
                Hostname = hostName,
                ServiceName = service,
                Urls = urls
            });

            var ev = new ServerDiscoveredEventArgs()
            {
                Hostname = hostName,
                ServiceName = service,
                Urls = urls

            };
            lock (_sync)
            {
                try
                {
                    _serviceFound.Invoke(this, ev);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ServiceFound event thrown exception.");
                }

                _services.Add(ev);
            }
        }

    }
}

