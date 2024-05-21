using Makaretu.Dns;

namespace EventPi.Advertiser.Receiver;

public class ServerDiscoveredEventArgs : EventArgs
{
    public required HostName Hostname { get; init; }
    public required ServiceName ServiceName { get; init; }
    public required IDictionary<InterfaceType, Uri> Urls { get; init; }
    public required IReadOnlyDictionary<string, string> Properties { get; init; }
    public Uri Url
    {
        get
        {
            if (Urls.TryGetValue(InterfaceType.Ethernet, out var u)) return u;
            return Urls.First().Value;
        }
    }
}