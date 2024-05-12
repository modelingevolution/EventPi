using CommunityToolkit.Mvvm.ComponentModel;
using EventPi.Abstractions;
using EventPi.Services.NetworkMonitor.Contract;
using MicroPlumberd;

namespace EventPi.Services.NetworkMonitor.Ui;

[EventHandler]
[INotifyPropertyChanged]
internal partial class WirelessConnectionVm(IPlumber plumber) : IAsyncDisposable
{
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private WirelessConnectivityState _connectivity = new WirelessConnectivityState();
    private bool _isInitialized = false;
    public WirelessConnectivityState Connectivity
    {
        get => _connectivity;
        private set => SetProperty(ref _connectivity, value);
    }


    private async Task Given(Metadata m, WirelessConnectivityState state) => Connectivity = state;

    public async Task<WirelessConnectionVm> Initialize(HostName hostName)
    {
        if (_isInitialized) return this;
        _isInitialized = true;
        var fullSn = WirelessConnectivityState.FullStreamName(hostName);
        await plumber.Subscribe(fullSn, FromRelativeStreamPosition.End - 1, cancellationToken: _cts.Token)
            .WithSnapshotHandler(this);
        return this;
    }

    public async ValueTask DisposeAsync()
    {
        await _cts.CancelAsync();
    }
}