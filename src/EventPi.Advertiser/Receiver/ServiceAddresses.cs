using System.Collections;

namespace EventPi.Advertiser.Receiver;

public record ServiceAddress : IEnumerable<KeyValuePair<InterfaceType, Uri>>
{
    public required HostName Hostname { get; init; }
    public required ServiceName ServiceName { get; init; }
    public required IReadOnlyDictionary<InterfaceType, Uri> Urls { get; init; }
    public IEnumerator<KeyValuePair<InterfaceType, Uri>> GetEnumerator() => Urls.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}