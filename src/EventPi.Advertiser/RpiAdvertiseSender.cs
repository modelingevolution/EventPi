
using Makaretu.Dns;

namespace EventPi.Advertiser;

public class RpiAdvertiseSender
{
    private ServiceProfile _serviceProfile;
    private readonly ServiceDiscovery _discoverService;

    private RpiAdvertiseSender(ServiceProfile serviceProfile, ServiceDiscovery discovery)
    {
        _serviceProfile = serviceProfile;
        _discoverService = discovery;
    }

    public static RpiAdvertiseSender Create(string instanceName,string serviceName, int port)
    {
        
        var profile = new ServiceProfile(instanceName, serviceName, (ushort)port);
        profile.AddWifiAndEthernetAddressesToProfile();
        var sender = new RpiAdvertiseSender(profile, new ServiceDiscovery());
        return sender;
    }

  // public void Advertise()
  // {
  //     _discoverService.Advertise(_serviceProfile);
  // }

    public void Advertise()
    {
        var discoverService = new ServiceDiscovery();
        discoverService.Advertise(new ServiceProfile("Super Raspberry","video.tcp.local", 6000));

    }

}