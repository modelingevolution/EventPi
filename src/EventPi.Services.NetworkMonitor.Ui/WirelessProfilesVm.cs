using CommunityToolkit.Mvvm.ComponentModel;
using EventPi.Abstractions;
using EventPi.Services.NetworkMonitor.Contract;
using MicroPlumberd;
using MudBlazor;
using Metadata = MicroPlumberd.Metadata;

namespace EventPi.Services.NetworkMonitor.Ui;

[EventHandler]
[INotifyPropertyChanged]
internal partial class WirelessProfilesVm(IPlumber plumber, ICommandBus commandBus) : IAsyncDisposable
{
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private WirelessProfilesState _profiles = new WirelessProfilesState();
    private bool _isInitialized = false;
    private HostName _hostName;

    public WirelessProfilesState Profiles
    {
        get => _profiles;
        private set => SetProperty(ref _profiles, value);
    }

    public string Style(WirelessProfile st)
    {
        if (st.IsConnected)
            return $"font-weight:bold; color:{Theme?.PaletteDark?.SuccessLighten}";
        return string.Empty;
    }
    private async Task Given(Metadata m, WirelessProfilesState state) => Profiles = state;
    private MudTheme Theme { get; set; } = new MudTheme();
    public async Task<WirelessProfilesVm> Initialize(HostName hostName)
    {
        if (_isInitialized) return this;
        _hostName = hostName;
        _isInitialized = true;
        var fullSn = WirelessProfilesState.FullStreamName(hostName);
        await plumber.Subscribe(fullSn, FromRelativeStreamPosition.End - 1, cancellationToken: _cts.Token)
            .WithSnapshotHandler(this);
        return this;
    }
    public string SearchString { get; set; }
    public Func<WirelessProfile, bool> Filter => x =>
    {
        if (string.IsNullOrWhiteSpace(SearchString))
            return true;

        if (x.Ssid.Contains(SearchString, StringComparison.OrdinalIgnoreCase))
            return true;

        if (x.InterfaceName.Contains(SearchString, StringComparison.OrdinalIgnoreCase))
            return true;

        if (x.ProfileName.Contains(SearchString, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    };
    public async ValueTask DisposeAsync()
    {
        await _cts.CancelAsync();
    }

    public async Task Delete(WirelessProfile p) => await commandBus.SendAsync(_hostName, new DeleteWirelessProfile() { ProfileId = p.Id });
    public async Task Activate(WirelessProfile p) => await commandBus.SendAsync(_hostName, new ActivateWirelessProfile() { ProfileId = p.Id });
    public async Task Disconnect(WirelessProfile p) => await commandBus.SendAsync(_hostName, new DisconnectWirelessNetwork()
    {
        ProfileId = p.Id, 
        Ssid = p.Ssid
    });

    

}