using CommunityToolkit.Mvvm.ComponentModel;
using EventPi.Abstractions;
using EventPi.Services.NetworkMonitor.Contract;
using MicroPlumberd;

namespace EventPi.Services.NetworkMonitor.Ui;

[EventHandler]
[INotifyPropertyChanged]
internal partial class WirelessStationsVm(IPlumber plumber) : IAsyncDisposable
{
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private WirelessStationsState _stations = new WirelessStationsState();
    private bool _isInitialized = false;
    public WirelessStationsState Stations
    {
        get => _stations;
        private set => SetProperty(ref _stations, value);
    }
        

    private async Task Given(Metadata m, WirelessStationsState state) => Stations = state;

    public async Task Initialize(string hostName)
    {
        if (_isInitialized) return;
        _isInitialized = true;
        await plumber.Subscribe(WirelessStationsState.FullStreamName((HostName)hostName), FromRelativeStreamPosition.End - 1, cancellationToken:_cts.Token)
            .WithSnapshotHandler(this);

    }

    public async ValueTask DisposeAsync()
    {
        await _cts.CancelAsync();
    }
}