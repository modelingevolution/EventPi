using EventPi.Abstractions;
using MicroPlumberd;

namespace EventPi.Services.Camera.Cli;

[EventHandler]
public partial class CameraWeldingConfigurationModel
{
    public int WeldingBorder { get; private set; }
    public int NonWeldingBorder { get; private set; }
    public int WeldingDataBufferSize => _brightBuffer.Capacity;

    private CircularBuffer<float> _brightBuffer;
    private CircularBuffer<float> _darkBuffer;

    private object _lock = new object();

    public CameraWeldingConfigurationModel()
    {
        _brightBuffer = new CircularBuffer<float>(1);
        _darkBuffer = new CircularBuffer<float>(1);
    }

    public float GetBrightPixels()
    {
        lock (_lock)
        {
            return _brightBuffer.Average();
        }
     
    }
    public float GetDarkPixels()
    {
        lock (_lock)
        {
            return _darkBuffer.Average();
        }
      
    }

    public void UpdateBuffers(FrameFeaturesRecord data)
    {
       
        _brightBuffer.AddLast(data.TotalBrightPixels);
        _darkBuffer.AddLast(data.TotalDarkPixels);

    }
    private async Task Given(Metadata m, CameraWeldingParamsDefined ev)
    {
        WeldingBorder = ev.WeldingBorder;
        NonWeldingBorder = ev.NonWeldingBorder;
        if (WeldingDataBufferSize != ev.WeldingDataBufferSize)
        {
            _darkBuffer = new CircularBuffer<float>(ev.WeldingDataBufferSize); 
            _brightBuffer = new CircularBuffer<float>(ev.WeldingDataBufferSize);

        }
       
    }

}