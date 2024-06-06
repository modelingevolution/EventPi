using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace EventPi.Services.Camera.Contract;

public record SetWeldingRecognitionConfiguration : INotifyPropertyChanged, IWeldingRecognitionConfigurationParameters
{
    private int _weldingValue;
    private int _nonWeldingValue;
    private bool _detectionEnabled;
    private int _brightPixelsBorder;
    private int _darkPixelsBorder;
    public Guid Id { get; set; } = Guid.NewGuid();

    public int WeldingValue
    {
        get => _weldingValue;
        set => SetField(ref _weldingValue, value);
    }

    public int NonWeldingValue
    {
        get => _nonWeldingValue;
        set => SetField(ref _nonWeldingValue, value);
    }

    public bool DetectionEnabled
    {
        get => _detectionEnabled;
        set => SetField(ref _detectionEnabled, value);
    }

    public int BrightPixelsBorder
    {
        get => _brightPixelsBorder;
        set => SetField(ref _brightPixelsBorder, value);
    }

    public int DarkPixelsBorder
    {
        get => _darkPixelsBorder;
        set => SetField(ref _darkPixelsBorder, value);
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
    public SetWeldingRecognitionConfiguration CopyFrom(IWeldingRecognitionConfigurationParameters src, bool raiseChange = false)
    {
        _brightPixelsBorder = src.BrightPixelsBorder;
        _darkPixelsBorder = src.DarkPixelsBorder;
        _detectionEnabled = src.DetectionEnabled;
        _nonWeldingValue = src.NonWeldingValue;
        _weldingValue = src.WeldingValue;
        
        if (raiseChange)
            this.OnPropertyChanged();
        return this;
    }
}