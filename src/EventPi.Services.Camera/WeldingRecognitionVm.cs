using EventPi.Services.Camera.Contract;
using MicroPlumberd;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using EventPi.Abstractions;

namespace EventPi.Services.Camera;

[EventHandler]
public partial class WeldingRecognitionVm : INotifyPropertyChanged, IAsyncDisposable
{
    private readonly ICommandBus _bus;
    private bool _initialized;
    private string _hostName;
    private int _cameraNr;
    private WeldingRecognitionConfigurationState? _prv;
    private SetWeldingRecognitionConfiguration _setWeldingRecognitionConfiguration = new SetWeldingRecognitionConfiguration();
    private DefineWeldingRecognitionConfiguration _defineWeldingRecognitionConfiguration = new DefineWeldingRecognitionConfiguration();
    public SetWeldingRecognitionConfiguration SetWeldingRecognitionConfiguration => _setWeldingRecognitionConfiguration;
    public DefineWeldingRecognitionConfiguration DefineWeldingRecognitionConfiguration => _defineWeldingRecognitionConfiguration;
    private ISubscriptionRunner? _weldingRecognitionConfigSub;
    private readonly IPlumber _plumber;
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly Channel<SetWeldingRecognitionConfiguration> _channel;
    public WeldingRecognitionVm(ICommandBus bus, IPlumber plumber)
    {
        _bus = bus;
        _plumber = plumber;
        _channel = Channel.CreateBounded<SetWeldingRecognitionConfiguration>(new BoundedChannelOptions(1) { FullMode = BoundedChannelFullMode.DropOldest });
        Task.Factory.StartNew(OnSendCommand, TaskCreationOptions.LongRunning);
    }

    private async Task OnSendCommand()
    {

        try
        {
            while (!_cts.IsCancellationRequested)
            {
                var cmd = await _channel.Reader.ReadAsync(_cts.Token);
                if (_hostName != null)
                    await _bus.SendAsync(WeldingRecognitionConfigurationState.StreamId(_hostName, _cameraNr), cmd);
            }
        }
        catch (OperationCanceledException) { }
    }

    private void SetWeldingRecognitionConfig()
    {
        var dto = _setWeldingRecognitionConfiguration with { Id = Guid.NewGuid() };
        _channel.Writer.WriteAsync(dto);
    }
    public async Task Save()
    {
        var dto = _defineWeldingRecognitionConfiguration.CopyFrom(SetWeldingRecognitionConfiguration);
        await _bus.SendAsync(_hostName, dto);
    }
    public async Task Cancel()
    {
        if (_prv == null) return;
        this.SetWeldingRecognitionConfiguration.CopyFrom(_prv);
        SetWeldingRecognitionConfig();
    }

    private async Task Given(Metadata m, WeldingRecognitionConfigurationState ev)
    {
        _prv = ev;
        this.SetWeldingRecognitionConfiguration.CopyFrom(ev, true);
        OnPropertyChanged("Command");
    }

    public async Task Initialize(string hostName, int cameraNr)
    {
        if (_initialized)
        {
            if (_hostName != hostName && _cameraNr != cameraNr)
                throw new InvalidOperationException();
            return;
        }
        _hostName = hostName;
        _cameraNr = cameraNr;
        _initialized = true;

        await(_weldingRecognitionConfigSub = _plumber.Subscribe(WeldingRecognitionConfigurationState.FullStreamName(HostName.From(_hostName), _cameraNr), FromRelativeStreamPosition.End - 1, cancellationToken: _cts.Token))
            .WithSnapshotHandler(this);
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public async ValueTask DisposeAsync()
    {
        await _cts.CancelAsync();
        //if(_camParametersSub != null)
        //    await _camParametersSub.DisposeAsync();
        if (_weldingRecognitionConfigSub != null)
            await _weldingRecognitionConfigSub.DisposeAsync();
    }
}