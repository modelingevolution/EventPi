using Makaretu.Dns;
using System.Text.Json;
using Zeroconf;

namespace EventPi.Advertiser.Receiver;

public class ServeDiscoveredEventArgs : EventArgs
{

}

public interface ILocalDiscoveryService
{
    public IEnumerable<ServiceAddresses> GetServices(string? serviceName = null);
    event EventHandler<ServeDiscoveredEventArgs> ServiceFound;
    event EventHandler<ServeDiscoveredEventArgs> ServiceLost;
}

class LocalDiscoveryService : ILocalDiscoveryService
{

    private ZeroconfResolver.ResolverListener _listener;
    private string _pathToSavedServices;
    private List<ServiceAddresses> _savedRpis;

    public IEnumerable<ServiceAddresses> GetServices(string? serviceName = null)
    {
        throw new NotImplementedException();
    }

    public event EventHandler<ServeDiscoveredEventArgs> ServiceFound;
    public event EventHandler<ServeDiscoveredEventArgs> ServiceLost;
    private object _sync = new object();

    private LocalDiscoveryService(ZeroconfResolver.ResolverListener listener)
    {
        _listener = listener;
    }

    public static LocalDiscoveryService Create(string serviceName, string pathToSavedServicesFile)
    {
        var listener = new LocalDiscoveryService(ZeroconfResolver.CreateListener(serviceName, 2000));
        listener.Init(pathToSavedServicesFile);
        return listener;

    }

    public void Init(string pathToSavedServicesFile)
    {
        _pathToSavedServices = pathToSavedServicesFile;
        _listener.ServiceFound += AddService;
        _listener.ServiceLost += LostService;
    }

    public void LostService(object sender, IZeroconfHost host)
    {

    }

    public void UpdateSavedRpis(ServiceAddresses ev, string wifiAddress, string ethernetAddress)
    {
        lock (_sync)
        {
            _savedRpis.Add(ev);
            var jsonString = JsonSerializer.Serialize(_savedRpis);
            File.WriteAllText(_pathToSavedServices, jsonString);
        }
    }
    public ServiceAddresses? TryUpdateSavedService(HostName hostname, string ethernetAddress, string wifiAddress, int port)
    {
        lock (_sync)
        {
            UpdateExitingServices(hostname, ethernetAddress, port);
        }

        var newService = new ServiceAddresses()
        {
            Hostname = hostname,
            ServiceName = 
        }
        AddNewService(hostname, ethernetAddress, port);
   
        UpdateSavedRpis(ev, wifiAddress, ethernetAddress);
        return ev;

    }

    private void UpdateExitingServices(HostName hostname, string ethernetAddress, int port)
    {
        for (var index = 0; index < _savedRpis.Count; index++)
        {
            if (_savedRpis[index].Hostname == hostname)
            {
                if (!_savedRpis[index].Urls.TryAdd(InterfaceType.Ethernet, new Uri(ethernetAddress + ":" + port)))
                {
                    _savedRpis[index].Urls[InterfaceType.Ethernet] = new Uri(ethernetAddress + ":" + port);
                }
            }
        }
    }

    public void AddService(object sender, IZeroconfHost host)
    {

        var srvs = host.Services;
        var properties = srvs.Values.First().Properties;
        

        RpiAdvertiseTools.GetWifiAndEthernet(properties, out string wifiAddress, out string ethernetAddress);
        var ev = TryUpdateSavedService((HostName)host.DisplayName, ethernetAddress, wifiAddress, srvs.Values.First().Port);

        if (ev != null)
        {
            ServiceFound.Invoke();
        }

    }

}