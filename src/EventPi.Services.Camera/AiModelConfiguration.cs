using System.ComponentModel;
using System.Runtime.CompilerServices;
using MicroPlumberd;
using ModelingEvolution.VideoStreaming;

namespace EventPi.Services.Camera;

[EventHandler]
public partial class AiModelConfiguration : INotifyPropertyChanged
{
    private AiModelConfigurationState _configurationState = new();
    public required VideoAddress VideoAddress { get; init; }

    public AiModelConfigurationState ConfigurationState
    {
        get => _configurationState;
        private set => SetField(ref _configurationState, value);
    }

    private async Task Given(Metadata m, AiModelConfigurationState ev)
    {
        ConfigurationState = ev;
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
}