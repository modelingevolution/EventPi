using Tmds.DBus.Protocol;

namespace NetworkManager.DBus;

record VethProperties
{
    internal ObjectPath Peer { get; set; } = default!;
}