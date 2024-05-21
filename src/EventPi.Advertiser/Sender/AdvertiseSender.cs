using Makaretu.Dns;

namespace EventPi.Advertiser.Sender;

public class AdvertiseSender
{
    private readonly ServiceProfile _serviceProfile;
    private readonly ServiceDiscovery _discoverService;
    public ServiceProfile Profile => _serviceProfile;

    private AdvertiseSender(ServiceProfile serviceProfile, ServiceDiscovery discovery)
    {
        _serviceProfile = serviceProfile;
        _discoverService = discovery;
        
    }

    public static AdvertiseSender Create(IServiceProfileEnricher profileEnricher, string instanceName, string schema,
        string serviceName, int port, params ServiceProperty[] properties)
    {
        var profile = new ServiceProfile(instanceName, serviceName, (ushort)port);
        profile.AddProperty("Schema", schema);
        profileEnricher.Enrich(profile);
        foreach(var property in properties)
        {
            profile.AddProperty(property.Key, property.Value);
        }
        
        var sender = new AdvertiseSender(profile, new ServiceDiscovery());
        return sender;
    }

    public void Advertise() => _discoverService.Advertise(_serviceProfile);
}

public interface IServiceProfileEnricher
{
    void Enrich(ServiceProfile profile);
}

class WifiAndEthernetEnricher : IServiceProfileEnricher
{
    public void Enrich(ServiceProfile profile)
    {
        profile.AddWifiAndEthernetAddressesToProfile();
    }
}