using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using EventPi.Services.Camera.Contract;
using MicroPlumberd;


namespace EventPi.Services.Camera;

[OutputStream("Camera")]
public record SetCameraParameters : ICameraParameters, INotifyPropertyChanged, ICameraParametersReadOnly
{
    private int _shutter;
    private float _analogueGain;
    private float _digitalGain;
    private float _contrast;
    private float _sharpness;
    private float _brightness;
    private float _blueGain;
    private float _redGain;
    public Guid Id { get; init; } = Guid.NewGuid();

    [Range(1, 40000)]
    public int Shutter
    {
        get => _shutter;
        set => SetField(ref _shutter, value);
    }

    [Range(0, 10)]
    public float AnalogueGain
    {
        get => _analogueGain;
        set => SetField(ref _analogueGain, value);
    }

    [Range(0, 10)]
    public float DigitalGain
    {
        get => _digitalGain;
        set => SetField(ref _digitalGain, value);
    }

    [Range(0, 10)]
    public float Contrast
    {
        get => _contrast;
        set => SetField(ref _contrast, value);
    }

    [Range(0, 1)]
    public float Sharpness
    {
        get => _sharpness;
        set => SetField(ref _sharpness, value);
    }

    [Range(-1, 1)]
    public float Brightness
    {
        get => _brightness;
        set => SetField(ref _brightness, value);
    }

    [Range(-1, 10)]
    public float BlueGain
    {
        get => _blueGain;
        set => SetField(ref _blueGain, value);
    }

    [Range(-1, 10)]
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
    public SetCameraParameters CopyFrom(ICameraParametersReadOnly src)
    {
        _analogueGain = src.AnalogueGain;
        _digitalGain = src.DigitalGain;
        _contrast = src.Contrast;
        _sharpness = src.Sharpness;
        _brightness = src.Brightness;
        _blueGain = src.BlueGain;
        _redGain = src.RedGain;
        _shutter = src.Shutter;
        return this;
    }

}