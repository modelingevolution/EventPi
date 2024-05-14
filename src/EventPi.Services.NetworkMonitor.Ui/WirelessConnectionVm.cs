using CommunityToolkit.Mvvm.ComponentModel;
using EventPi.Abstractions;
using EventPi.Services.NetworkMonitor.Contract;
using MicroPlumberd;
using MudBlazor;

namespace EventPi.Services.NetworkMonitor.Ui;

[EventHandler]
[INotifyPropertyChanged]
internal partial class WirelessConnectionVm(IPlumber plumber, IDialogService dialogService) : IAsyncDisposable
{
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private WirelessConnectivityState _connectivity = new WirelessConnectivityState();
    private bool _isInitialized = false;

    public WirelessConnectivityState Connectivity
    {
        get => _connectivity;
        private set => SetProperty(ref _connectivity, value);
    }
    // this is executed in main thread.
    private async Task OnProcessPwd(WirelessConnectivityState st)
    {
        var parameters = new DialogParameters<WirelessStationPwdDialog>
        {
            { x => x.InterfaceName, st.InterfaceName },
            { x => x.Ssid, st.Ssid },
            
        };
        DialogOptions op = new DialogOptions() { ClassBackground = "blur-background", MaxWidth = MaxWidth.Medium };
        var dialog = await dialogService.ShowAsync<WirelessStationPwdDialog>($"Wifi needs authentication", parameters, op);
    }

    public Func<Func<Task>, Task> InvokeAsync { get; set; }


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