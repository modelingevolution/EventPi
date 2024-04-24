using System.Collections;

namespace EventPi.Advertiser;

public record ServiceAddresses : IEnumerable<KeyValuePair<InterfaceType,Uri>>
{
    public  HostName Hostname { get; init; }
    public string ServiceName { get; init; }
    public IDictionary<InterfaceType, Uri> Urls { get; }


    public IEnumerator<KeyValuePair<InterfaceType, Uri>> GetEnumerator()
    {
        return Urls.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public enum InterfaceType
{
    Ethernet, Wifi
}