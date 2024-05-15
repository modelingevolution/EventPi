namespace NetworkManager.DBus;

record StatisticsProperties
{
    internal uint RefreshRateMs { get; set; } = default!;
    internal ulong TxBytes { get; set; } = default!;
    internal ulong RxBytes { get; set; } = default!;
}