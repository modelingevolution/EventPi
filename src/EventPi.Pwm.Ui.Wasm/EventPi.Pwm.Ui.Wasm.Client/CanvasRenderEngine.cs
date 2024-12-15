using System.Collections.Concurrent;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Numerics;
using EventPi.SignalProcessing;
using ModelingEvolution.VideoStreaming.Buffers;
using SkiaSharp;
using SkiaSharp.Views.Blazor;

namespace EventPi.Pwm.Ui.Wasm.Client
{
    public enum Align
    {
        Center, Bottom, ManualOffset
    }
    public class CanvasRenderEngine
    {
        private Size _size;
        
        // width
        private float _w; 
        private ulong _colWidth;
        // 2xwidth
        private float _2w;
        private float _dt = 5f;
        private float _h;
        private float _dy = 200;
        private ulong _col = 0;
        private SortedListDictionary<ushort, object>? _prv;
        private SKBitmap? _bitmap;
        private SKCanvas? _buffer;
        private readonly ISignalsStream _stream;
        private readonly ConcurrentQueue<Action> _events;
        public CanvasRenderEngine(ISignalsStream stream, Size? size = null)
        {
            _stream = stream;
            _events = new();
            Size = size ?? new Size(640, 640);
            
        }

        public float Dt => _dt;
        public Align Align { get; set; }
        public Vector2 Offset { get; set; }
        public Size Size
        {
            get => _size;
            set
            {
                if (value == _size) return;
                _size = value;
                
                Invoke(Resize);
            }
        }
        public void Invoke(Action a) => _events.Enqueue(a);
        public void Resize()
        {
            var value = Size;
            _w = value.Width;
            _colWidth = (ulong)(_w / _dt); // celling?
            _2w = _w * 2;
            _h = value.Height;
            _col = 0;
            _buffer?.Dispose();
            _bitmap?.Dispose();
            
            _bitmap = new SKBitmap((int)MathF.Ceiling(_2w), (int)MathF.Ceiling(_h));
            _buffer = new SKCanvas(_bitmap);

            if (Align == Align.Center)
                Offset = new Vector2(0, -_h / 2);
            else if (Align == Align.Bottom)
                Offset = new Vector2(0, 0);
        }
        public void Paint(SKPaintSurfaceEventArgs args)
        {
            this.Paint(args.Surface.Canvas);
        }
        private void DrawSignal(SKCanvas canvas, ushort id, float prvX, object prvValue, float x, object currentValue)
        {
            var prvY = (float)prvValue;
            prvY *= _dy;
            var h = _h;
            prvY = h - prvY;
            var currentY = (float)currentValue;
            currentY *= _dy;
            currentY = h - currentY;

            using var red = new SKPaint() { Color = SKColors.Red, StrokeWidth = 2, Style = SKPaintStyle.Stroke };
            canvas.DrawLine(prvX, prvY, x, currentY, red);
            //Console.WriteLine($"Draw: [{prvX} {prvY}] [{x} {currentY}]");
        }
        public void Paint(SKCanvas c)
        {
            while (_events.TryDequeue(out var a)) a();
            
            
            _buffer!.Save();
            _buffer.Translate(Offset.X, Offset.Y);
            float leftOffset = 0f;
            int processed = 0;
            while (_stream?.TryRead(out var result) ?? false)
            {
                processed +=1;
                if (_prv == null)
                {
                    _prv = result!;
                    _col += 1;
                    continue;
                }

                float prvX = CalculateX(_col - 1);
                float x = CalculateX(_col);
                if (x < prvX)
                {
                    // We have a switch.
                    // Somewhere here 1 bugs:
                    // It seems there is a bit too big one chunk...
                    
                    prvX = _w;
                    x = CalculateX(++_col);

                    _buffer.Restore();
                    using var transparent = new SKPaint() 
                    { 
                        Color = SKColors.Transparent, 
                        Style = SKPaintStyle.Fill,
                        BlendMode = SKBlendMode.Clear
                    };
                    var rightSectionOfCanvas = new SKRect(_w, 0, _2w, _h);
                    var leftSectionOfCanvas = new SKRect(0, 0, _w, _h);
                    _buffer.DrawRect(leftSectionOfCanvas, transparent);
                    //_buffer.Flush();
                    
                    _buffer.DrawBitmap(_bitmap, rightSectionOfCanvas, leftSectionOfCanvas);
                    //_buffer.Flush();
                    
                    _buffer.DrawRect(rightSectionOfCanvas,transparent);
                    //_buffer.Flush();

                    _buffer!.Save();
                    _buffer.Translate(Offset.X, Offset.Y);
                }

                if (x > _w)
                    leftOffset = x - _w;


                foreach (var s in result!)
                {
                    if (_prv.TryGetValue(s.Key, out var prvValue))
                    {
                        DrawSignal(_buffer, s.Key, prvX, prvValue, x, s.Value);
                    }
                }
                _prv.Dispose();
                _prv = result;
                _col += 1;
            }
            
            _buffer.Flush();
            _buffer.Restore();

            if (processed == 0) return;
            
            var src = new SKRect(leftOffset, 0, leftOffset + _w, _h);
            using var paint = new SKPaint();
            paint.BlendMode = SKBlendMode.Src;
            c.DrawBitmap(_bitmap, src, Rect, paint);
            c.DrawTextBox(0, 0, 12, 100, $"{_col}/{processed}", SKColors.LightGray, SKColors.Black);

        }

        private SKRect Rect => new SKRect(0, 0, _w, _h);
        private float CalculateX(ulong col)
        {
            if (col <= _colWidth)
                return col * _dt;
            
            var rem = col % _colWidth;
            var x = rem * _dt;

            if (rem == 0) return _2w;
            return _w + x;
        }

    }
}
