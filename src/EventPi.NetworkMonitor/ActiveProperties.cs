using Tmds.DBus.Protocol;

namespace NetworkManager.DBus;

record ActiveProperties
{
    internal ObjectPath Connection { get; set; } = default!;
    internal ObjectPath SpecificObject { get; set; } = default!;
    internal string Id { get; set; } = default!;
    internal string Uuid { get; set; } = default!;
    internal string Type { get; set; } = default!;
    internal ObjectPath[] Devices { get; set; } = default!;
    internal uint State { get; set; } = default!;
    internal uint StateFlags { get; set; } = default!;
    internal bool Default { get; set; } = default!;
    internal ObjectPath Ip4Config { get; set; } = default!;
    internal ObjectPath Dhcp4Config { get; set; } = default!;
    internal bool Default6 { get; set; } = default!;
    internal ObjectPath Ip6Config { get; set; } = default!;
    internal ObjectPath Dhcp6Config { get; set; } = default!;
    internal bool Vpn { get; set; } = default!;
    internal ObjectPath Controller { get; set; } = default!;
    internal ObjectPath Master { get; set; } = default!;
}