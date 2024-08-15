using Tmds.DBus.Protocol;

namespace NetworkManager.DBus;

record SettingsProperties
{
    internal ObjectPath[] Connections { get; set; } = default!;
    internal string Hostname { get; set; } = default!;
    internal bool CanModify { get; set; } = default!;
}