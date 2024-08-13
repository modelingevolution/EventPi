namespace NetworkManager.DBus;

record WiredProperties
{
    internal string HwAddress { get; set; } = default!;
    internal string PermHwAddress { get; set; } = default!;
    internal uint Speed { get; set; } = default!;
    internal string[] S390Subchannels { get; set; } = default!;
    internal bool Carrier { get; set; } = default!;
}