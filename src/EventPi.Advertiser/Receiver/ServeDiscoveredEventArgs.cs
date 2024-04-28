using Makaretu.Dns;

namespace EventPi.Advertiser.Receiver;

public class ServerDiscoveredEventArgs : EventArgs
{
    public HostName Hostname { get; init; }
    public string ServiceName { get; init; }
    public IDictionary<InterfaceType, Uri> Urls { get; init; }
}