using Tmds.DBus.Protocol;

namespace NetworkManager.DBus;

record WirelessProperties
{
    internal string HwAddress { get; set; } = default!;
    internal string PermHwAddress { get; set; } = default!;
    internal uint Mode { get; set; } = default!;
    internal uint Bitrate { get; set; } = default!;
    internal ObjectPath[] AccessPoints { get; set; } = default!;
    internal ObjectPath ActiveAccessPoint { get; set; } = default!;
    internal uint WirelessCapabilities { get; set; } = default!;
    internal long LastScan { get; set; } = default!;
}