using Tmds.DBus.Protocol;

namespace NetworkManager.DBus;

record IP6ConfigProperties
{
    internal (byte[], uint, byte[])[] Addresses { get; set; } = default!;
    internal Dictionary<string, VariantValue>[] AddressData { get; set; } = default!;
    internal string Gateway { get; set; } = default!;
    internal (byte[], uint, byte[], uint)[] Routes { get; set; } = default!;
    internal Dictionary<string, VariantValue>[] RouteData { get; set; } = default!;
    internal byte[][] Nameservers { get; set; } = default!;
    internal string[] Domains { get; set; } = default!;
    internal string[] Searches { get; set; } = default!;
    internal string[] DnsOptions { get; set; } = default!;
    internal int DnsPriority { get; set; } = default!;
}