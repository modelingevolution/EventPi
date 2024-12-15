using EventPi.Pwm.Ui.Wasm.Client;
using EventPi.SignalProcessing;
using SkiaSharp;

namespace EventPi.Tests;

public class CanvasRenderEngineTests
{
    private readonly SignalsQueueStream _stream;
    private readonly CanvasRenderEngine _sut;
    private readonly SKBitmap _bitmap;
    private readonly SKCanvas _canvas;
    public CanvasRenderEngineTests()
    {
        
        _stream = new();
        _sut = new CanvasRenderEngine(_stream);
        _bitmap = new SKBitmap(_sut.Size.Width, _sut.Size.Height);
        _canvas = new SKCanvas(_bitmap);
    }
    [Fact]
    public void Paint()
    {
        for(int i = 0; i < 640/_sut.Dt; i++)
            _stream.Write(1,(float)i);
        
        _sut.Paint(_canvas);
    }
    [Fact]
    public void Paint2()
    {
        for (int i = 0; i < 640 / _sut.Dt + 2; i++)
            _stream.Write(1, (float)i);

        _sut.Paint(_canvas);
    }
    [Fact]
    public void Paint3()
    {
        for (int i = 0; i < 2*640 / _sut.Dt + 2; i++)
            _stream.Write(1, (float)i);

        _sut.Paint(_canvas);
    }
}
