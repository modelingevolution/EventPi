using Makaretu.Dns;

namespace EventPi.Advertiser.Sender;

public class AdvertiseSender
{
    private readonly ServiceProfile _serviceProfile;
    private readonly ServiceDiscovery _discoverService;

    private AdvertiseSender(ServiceProfile serviceProfile, ServiceDiscovery discovery)
    {
        _serviceProfile = serviceProfile;
        _discoverService = discovery;
    }

    public static AdvertiseSender Create(string instanceName, string serviceName, int port)
    {
        var profile = new ServiceProfile(instanceName, serviceName, (ushort)port);
        profile.AddWifiAndEthernetAddressesToProfile();
        var sender = new AdvertiseSender(profile, new ServiceDiscovery());
        return sender;
    }

    public void Advertise() => _discoverService.Advertise(_serviceProfile);
}