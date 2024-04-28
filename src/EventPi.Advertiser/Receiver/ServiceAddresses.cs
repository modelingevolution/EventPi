using System.Collections;

namespace EventPi.Advertiser.Receiver;

public record ServiceAddresses : IEnumerable<KeyValuePair<InterfaceType, Uri>>
{
    public HostName Hostname { get; init; }
    public string ServiceName { get; init; }
    public IDictionary<InterfaceType, Uri> Urls { get; init; }


    public IEnumerator<KeyValuePair<InterfaceType, Uri>> GetEnumerator()
    {
        return Urls.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}