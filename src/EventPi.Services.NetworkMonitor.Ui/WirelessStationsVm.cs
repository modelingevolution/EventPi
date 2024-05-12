using CommunityToolkit.Mvvm.ComponentModel;
using EventPi.Abstractions;
using EventPi.Services.NetworkMonitor.Contract;
using MicroPlumberd;

namespace EventPi.Services.NetworkMonitor.Ui;

[EventHandler]
[INotifyPropertyChanged]
internal partial class WirelessStationsVm(IPlumber plumber, ICommandBus bus) : IAsyncDisposable
{
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private WirelessStationsState _stations = new WirelessStationsState();
    private bool _isInitialized = false;
    private HostName _hostName;

    public WirelessStationsState Stations
    {
        get => _stations;
        private set => SetProperty(ref _stations, value);
    }

    public string Style(WirelessStation st)
    {
        return string.Empty;
    }
    public string SearchString { get; set; }
    public Func<WirelessStation, bool> Filter => x =>
    {
        if (string.IsNullOrWhiteSpace(SearchString))
            return true;

        if (x.Ssid.Contains(SearchString, StringComparison.OrdinalIgnoreCase))
            return true;

        if (x.InterfaceName.Contains(SearchString, StringComparison.OrdinalIgnoreCase))
            return true;


        return false;
    };

    private async Task Given(Metadata m, WirelessStationsState state) => Stations = state;

    public async Task<WirelessStationsVm> Initialize(HostName hostName)
    {
        if (_isInitialized) return this;
        _hostName = hostName;
        _isInitialized = true;
        var fullSn = WirelessStationsState.FullStreamName(hostName);
        await plumber.Subscribe(fullSn, FromRelativeStreamPosition.End - 1, cancellationToken:_cts.Token)
            .WithSnapshotHandler(this);
        return this;
    }

    public async ValueTask DisposeAsync()
    {
        await _cts.CancelAsync();
    }

    public Task RequestRefresh()
    {
        return bus.SendAsync(_hostName, new RequestWifiScan());
    }
}