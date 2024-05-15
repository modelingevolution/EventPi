using Tmds.DBus.Protocol;

namespace NetworkManager.DBus;

record DeviceProperties
{
    internal string Udi { get; set; } = default!;
    internal string Path { get; set; } = default!;
    internal string Interface { get; set; } = default!;
    internal string IpInterface { get; set; } = default!;
    internal string Driver { get; set; } = default!;
    internal string DriverVersion { get; set; } = default!;
    internal string FirmwareVersion { get; set; } = default!;
    internal uint Capabilities { get; set; } = default!;
    internal uint Ip4Address { get; set; } = default!;
    internal uint State { get; set; } = default!;
    internal (uint, uint) StateReason { get; set; } = default!;
    internal ObjectPath ActiveConnection { get; set; } = default!;
    internal ObjectPath Ip4Config { get; set; } = default!;
    internal ObjectPath Dhcp4Config { get; set; } = default!;
    internal ObjectPath Ip6Config { get; set; } = default!;
    internal ObjectPath Dhcp6Config { get; set; } = default!;
    internal bool Managed { get; set; } = default!;
    internal bool Autoconnect { get; set; } = default!;
    internal bool FirmwareMissing { get; set; } = default!;
    internal bool NmPluginMissing { get; set; } = default!;
    internal uint DeviceType { get; set; } = default!;
    internal ObjectPath[] AvailableConnections { get; set; } = default!;
    internal string PhysicalPortId { get; set; } = default!;
    internal uint Mtu { get; set; } = default!;
    internal uint Metered { get; set; } = default!;
    internal Dictionary<string, VariantValue>[] LldpNeighbors { get; set; } = default!;
    internal bool Real { get; set; } = default!;
    internal uint Ip4Connectivity { get; set; } = default!;
    internal uint Ip6Connectivity { get; set; } = default!;
    internal uint InterfaceFlags { get; set; } = default!;
    internal string HwAddress { get; set; } = default!;
    internal ObjectPath[] Ports { get; set; } = default!;
}