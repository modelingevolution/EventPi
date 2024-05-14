using CommunityToolkit.Mvvm.ComponentModel;
using EventPi.Abstractions;
using EventPi.Services.NetworkMonitor.Contract;
using MicroPlumberd;
using MudBlazor;

namespace EventPi.Services.NetworkMonitor.Ui;

[EventHandler]
[INotifyPropertyChanged]
internal partial class WirelessStationsVm(IPlumber plumber, ICommandBus bus, IDialogService dialogService) : IAsyncDisposable
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

    public async Task<bool> Connect(WirelessStation st)
    {
        while(true)
        try
        {
            await bus.SendAsync(_hostName, new ConnectAccessPoint() { Ssid = st.Ssid });
            return true;
        }
        catch (FaultException<ConnectionError> ex)
        {
            if (ex.Data.Reason == ConnectionErrorReason.MissingProfile)
            {
                var parameters = new DialogParameters<WirelessStationPwdDialog>
                    {
                        { x => x.InterfaceName, st.InterfaceName },
                        { x => x.Ssid, st.Ssid }
                    };
                DialogOptions op = new DialogOptions() { ClassBackground = "blur-background", MaxWidth = MaxWidth.Medium };
                var dialog = await dialogService.ShowAsync<WirelessStationPwdDialog>($"Wifi needs authentication", parameters, op);
                var result = await dialog.Result;
                if (result.Canceled)
                    return false;
            }
            else
            {
                await dialogService.ShowMessageBox("Connection error", ex.Data.Message);
                return false;
            }
        }
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