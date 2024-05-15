using Tmds.DBus.Protocol;

namespace NetworkManager.DBus;

record IP4ConfigProperties
{
    internal uint[][] Addresses { get; set; } = default!;
    internal Dictionary<string, VariantValue>[] AddressData { get; set; } = default!;
    internal string Gateway { get; set; } = default!;
    internal uint[][] Routes { get; set; } = default!;
    internal Dictionary<string, VariantValue>[] RouteData { get; set; } = default!;
    internal Dictionary<string, VariantValue>[] NameserverData { get; set; } = default!;
    internal uint[] Nameservers { get; set; } = default!;
    internal string[] Domains { get; set; } = default!;
    internal string[] Searches { get; set; } = default!;
    internal string[] DnsOptions { get; set; } = default!;
    internal int DnsPriority { get; set; } = default!;
    internal string[] WinsServerData { get; set; } = default!;
    internal uint[] WinsServers { get; set; } = default!;
}