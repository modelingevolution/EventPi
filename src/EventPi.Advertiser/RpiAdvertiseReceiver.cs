using Makaretu.Dns;
using System.Text.Json;
using Zeroconf;

namespace EventPi.Advertiser;

public class ServeDiscoveredEventArgs : EventArgs
{

}

public interface ILocalDiscoveryService
{
    public IEnumerable<IServiceDiscoveryInfo> GetServices(string? serviceName = null);
    event EventHandler<ServeDiscoveredEventArgs> ServiceFound;
    event EventHandler<ServeDiscoveredEventArgs> ServiceLost;
}

class LocalDiscoveryService : ILocalDiscoveryService
{

    private ZeroconfResolver.ResolverListener _listener;
    private string _pathToSavedServices;
    private List<IServiceDiscoveryInfo> _savedRpis;
    public IEnumerable<IServiceDiscoveryInfo> GetServices(string? serviceName)
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
        this._pathToSavedServices = pathToSavedServicesFile;
        this._listener.ServiceFound += AddService;
        this._listener.ServiceLost += LostService;
    }
   
    public void LostService(object sender, IZeroconfHost host)
    {

    }

    public void UpdateSavedRpis(IServiceDiscoveryInfo ev, string wifiAddress, string ethernetAddress)
    {
        lock (_sync)
        {
            _savedRpis.Add(ev);
            var jsonString = JsonSerializer.Serialize(_savedRpis);
            File.WriteAllText(_pathToSavedServices, jsonString);
        }
    }
    public IServiceDiscoveryInfo? TryUpdateSavedService(HostName hostname, string ethernetAddress, string wifiAddress, int port)
    {
        lock (_sync)
        {
            if (_savedRpis.Exists(x => x.Hostname == hostname && x.IPAddressEth0 == ethernetAddress && x.IPAddressWlan0 == wifiAddress))
            {
                return null;
            }
        }

        IServiceDiscoveryInfo ev = new ServiceDiscoveryInfo()
        {
            Hostname = hostname,
            IPAddressEth0 = ethernetAddress,
            IPAddressWlan0 = wifiAddress,
            Port = port

        };
        
        UpdateSavedRpis(ev, wifiAddress, ethernetAddress);
        return ev;
            
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