﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using EventPi.Services.Camera.Contract;
using MicroPlumberd;

namespace EventPi.Services.Camera.Ui;

[EventHandler]
public partial class CameraControlsVm : INotifyPropertyChanged,  IAsyncDisposable
{
    private CameraProfile? _prv;
    
    private readonly SetCameraParameters _setCameraParameters = new SetCameraParameters();
    private readonly DefineProfileConfiguration _defineProfileConfiguration = new();
    private string? _hostName;
    private ISubscriptionRunner? _camParametersSub;
    private ISubscriptionRunner? _camProfileSub;
    public SetCameraParameters SetCameraParameters => _setCameraParameters;
    public DefineProfileConfiguration DefineProfileConfiguration => _defineProfileConfiguration;
    private readonly Channel<SetCameraParameters> _channel;
    private readonly IPlumber _plumber;
    private readonly ICommandBus _bus;
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private string _profileName;
    

    public CameraControlsVm(IPlumber plumber, ICommandBus bus)
    {
        _plumber = plumber;
        _bus = bus;
        _channel = Channel.CreateBounded<SetCameraParameters>(new BoundedChannelOptions(1){ FullMode = BoundedChannelFullMode.DropOldest});
        Task.Factory.StartNew(OnSendCommand, TaskCreationOptions.LongRunning);
        _setCameraParameters.PropertyChanged += OnSetCameraParametersPropertyChanged;
    }

    private async Task OnSendCommand()
    {
        try
        {
            while (!_cts.IsCancellationRequested)
            {
                var cmd = await _channel.Reader.ReadAsync(_cts.Token);
                if (_hostName != null)
                    await _bus.SendAsync(CameraParametersState.StreamId(_hostName), cmd);
            }
        }
        catch(OperationCanceledException){}
    }

    private void SetCamera()
    {
        var dto = _setCameraParameters with { Id = Guid.NewGuid() };
        _channel.Writer.WriteAsync(dto);
    }

    public async Task Save()
    {
        var dto = _defineProfileConfiguration.CopyFrom(this.SetCameraParameters);
        await _bus.SendAsync(CameraProfile.StreamId(_hostName, _profileName), dto);
    }
    public async Task Cancel()
    {
        if (_prv == null) return;
        SetCameraParameters.CopyFrom(_prv);
        SetCamera();
    }
    public async Task Initialize(string hostName, string? profileName=null)
    {
        _hostName = hostName;
        _profileName = profileName ?? "default";
        _defineProfileConfiguration.Hostname = hostName;
        _defineProfileConfiguration.Profile = _profileName;
        
        await (_camParametersSub=_plumber.Subscribe(CameraParametersState.FullStreamName(_hostName), FromRelativeStreamPosition.End - 1))
            .WithSnapshotHandler(this);
        await (_camProfileSub = _plumber.Subscribe(CameraProfile.FullStreamName(_hostName, _profileName), FromRelativeStreamPosition.End - 1))
            .WithSnapshotHandler(this);
    }

    private void OnSetCameraParametersPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        SetCamera();
    }

    private async Task Given(Metadata m, CameraProfile ev)
    {
        _prv = ev;
        this.SetCameraParameters.CopyFrom(ev);
        OnPropertyChanged("Command");
    }
    private async Task Given(Metadata m, CameraParametersState ev)
    {
        this.SetCameraParameters.CopyFrom(ev);
        OnPropertyChanged("Command");
    }

    public event PropertyChangedEventHandler? PropertyChanged;

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
        if(_camParametersSub != null)
            await _camParametersSub.DisposeAsync();
        if (_camProfileSub != null)
            await _camProfileSub.DisposeAsync();
    }
}