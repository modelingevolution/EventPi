using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using MicroPlumberd;

namespace EventPi.Services.Camera.Contract;

[OutputStream("Camera")]
public record SetCameraParameters : ICameraParameters, INotifyPropertyChanged, ICameraParametersReadOnly
{
    public SetCameraParameters()
    {
        _shutter = 1;
        _colorMap = ColormapTypes.COLORMAP_NONE;
    }
    private int _shutter;
    private HdrModeEnum _hdrMode;
    private ColormapTypes _colorMap;
    private float _analogueGain;
    private float _exposureLevel;
    private float _digitalGain;
    private float _contrast;
    private float _sharpness;
    private float _brightness;
    private float _blueGain;
    private float _redGain;
    private bool _autoHistogramEnabled;
    public Guid Id { get; init; } = Guid.NewGuid();

    public ColormapTypes ColorMap
    {
        get => _colorMap;
        set => SetField(ref _colorMap, value);
    }
    public HdrModeEnum HdrMode
    {
        get => _hdrMode;
        set => SetField(ref _hdrMode, value);
    }

    [Range(1, 40000)]
    public int Shutter
    {
        get => _shutter;
        set => SetField(ref _shutter, value);
    }
    [Range(-16.0f, 16.0f)]
    public float ExposureLevel
    {
        get => _exposureLevel;
        set => SetField(ref _exposureLevel, value);
    }

    [Range(-16, 16)]
    public float AnalogueGain
    {
        get => _analogueGain;
        set => SetField(ref _analogueGain, value);
    }

    public bool AutoHistogramEnabled
    {
        get => _autoHistogramEnabled;
        set => SetField(ref _autoHistogramEnabled, value);
    }

    [Range(-16, 16)]
    public float DigitalGain
    {
        get => _digitalGain;
        set => SetField(ref _digitalGain, value);
    }

    [Range(-16, 16)]
    public float Contrast
    {
        get => _contrast;
        set => SetField(ref _contrast, value);
    }

    [Range(-1, 16)]
    public float Sharpness
    {
        get => _sharpness;
        set => SetField(ref _sharpness, value);
    }

    [Range(0,1)]
    public float Brightness
    {
        get => _brightness;
        set => SetField(ref _brightness, value);
    }

    [Range(-16, 16)]
    public float BlueGain
    {
        get => _blueGain;
        set => SetField(ref _blueGain, value);
    }

    [Range(-16, 16)]
    public float RedGain
    {
        get => _redGain;
        set => SetField(ref _redGain, value);
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
    public SetCameraParameters CopyFrom(ICameraParametersReadOnly src, bool raiseChange = false)
    {
        if(src==null) return this;
        _analogueGain = src.AnalogueGain;
        _hdrMode = src.HdrMode;
        _digitalGain = src.DigitalGain;
        _contrast = src.Contrast;
        _sharpness = src.Sharpness;
        _exposureLevel = src.ExposureLevel;
        _brightness = src.Brightness;
        _blueGain = src.BlueGain;
        _redGain = src.RedGain;
        _shutter = src.Shutter;
        _autoHistogramEnabled = src.AutoHistogramEnabled;
        if(raiseChange)
            this.OnPropertyChanged();
        return this;
    }

}