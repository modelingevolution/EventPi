using Tmds.DBus.Protocol;

namespace NetworkManager.DBus;

record BridgeProperties
{
    internal string HwAddress { get; set; } = default!;
    internal bool Carrier { get; set; } = default!;
    internal ObjectPath[] Slaves { get; set; } = default!;
}