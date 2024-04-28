using System.Collections;

namespace EventPi.Advertiser.Receiver;

public record ServiceAddresses : IEnumerable<KeyValuePair<InterfaceType, Uri>>
{
    public required HostName Hostname { get; init; }
    public required string ServiceName { get; init; }
    public required IReadOnlyDictionary<InterfaceType, Uri> Urls { get; init; }
    public IEnumerator<KeyValuePair<InterfaceType, Uri>> GetEnumerator() => Urls.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}