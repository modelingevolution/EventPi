namespace NetworkManager.DBus;

record AccessPointProperties
{
    internal uint Flags { get; set; } = default!;
    internal uint WpaFlags { get; set; } = default!;
    internal uint RsnFlags { get; set; } = default!;
    internal byte[] Ssid { get; set; } = default!;
    internal uint Frequency { get; set; } = default!;
    internal string HwAddress { get; set; } = default!;
    internal uint Mode { get; set; } = default!;
    internal uint MaxBitrate { get; set; } = default!;
    internal byte Strength { get; set; } = default!;
    internal int LastSeen { get; set; } = default!;
}