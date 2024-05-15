using EventPi.Abstractions;
using MicroPlumberd;

namespace EventPi.Services.NetworkMonitor.Contract;

[OutputStream("WirelessConnectivity")]
public record WirelessConnectivityState : IStatefulStream<HostName>
{
    public static string FullStreamName(HostName id) => $"WirelessConnectivity-{id}";
    public string? Ssid { get; init; }
    public string? InterfaceName { get; init; }
    public Ip4Config? IpConfig { get; init; }
        
    public string? ConnectionName { get; init; }
    public DeviceState State { get; init; }
    public ActivationFailedReason? FailedReason { get; init; }
    public byte Signal { get; init; }
}