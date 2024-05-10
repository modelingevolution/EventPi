using Tmds.DBus.Protocol;

namespace NetworkManager.DBus;

record NetworkManagerProperties
{
    internal ObjectPath[] Devices { get; set; } = default!;
    internal ObjectPath[] AllDevices { get; set; } = default!;
    internal ObjectPath[] Checkpoints { get; set; } = default!;
    internal bool NetworkingEnabled { get; set; } = default!;
    internal bool WirelessEnabled { get; set; } = default!;
    internal bool WirelessHardwareEnabled { get; set; } = default!;
    internal bool WwanEnabled { get; set; } = default!;
    internal bool WwanHardwareEnabled { get; set; } = default!;
    internal bool WimaxEnabled { get; set; } = default!;
    internal bool WimaxHardwareEnabled { get; set; } = default!;
    internal uint RadioFlags { get; set; } = default!;
    internal ObjectPath[] ActiveConnections { get; set; } = default!;
    internal ObjectPath PrimaryConnection { get; set; } = default!;
    internal string PrimaryConnectionType { get; set; } = default!;
    internal uint Metered { get; set; } = default!;
    internal ObjectPath ActivatingConnection { get; set; } = default!;
    internal bool Startup { get; set; } = default!;
    internal string Version { get; set; } = default!;
    internal uint[] VersionInfo { get; set; } = default!;
    internal uint[] Capabilities { get; set; } = default!;
    internal uint State { get; set; } = default!;
    internal uint Connectivity { get; set; } = default!;
    internal bool ConnectivityCheckAvailable { get; set; } = default!;
    internal bool ConnectivityCheckEnabled { get; set; } = default!;
    internal string ConnectivityCheckUri { get; set; } = default!;
    internal Dictionary<string, VariantValue> GlobalDnsConfiguration { get; set; } = default!;
}