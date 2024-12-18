using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using ModelingEvolution.VideoStreaming.Buffers;
using SkiaSharp;
using SkiaSharp.Views.Blazor;

namespace EventPi.SignalProcessing.Ui
{
    public enum Align
    {
        Center, Bottom, ManualOffset
    }

    public static class Extensions
    {
        public static ulong ToLongSeconds(this DateTime t)
        {
            return (ulong)t.ToSeconds();
        }
        public static double ToSeconds(this DateTime t)
        {
            return (t - DateTime.UnixEpoch).TotalSeconds;
        }
    }
    public class ColorGenerator
    {
        /// <summary>
        /// Generates an SKPaint object with a fully saturated color based on the given ID.
        /// </summary>
        /// <param name="id">The ID (0-15) for which to generate the color.</param>
        /// <returns>An SKPaint object with the corresponding color.</returns>
        public static SKColor GetPaintById(byte id, int count = 16)
        {
            // Ensure the ID is within the valid range (0-15)
            id = (byte)(id % count);
            // Calculate the hue (0-360 degrees divided into 16 parts)
            float hue = (id / (float)count) * 360f;
            // Convert HSV to RGB
            SKColor color = HsvToRgb(hue, 1f, 1f);
            // Create and return the SKPaint object
            return color;
        }
        /// <summary>
        /// Converts HSV values to an SKColor.
        /// </summary>
        /// <param name="hue">Hue (0-360 degrees).</param>
        /// <param name="saturation">Saturation (0-1).</param>
        /// <param name="value">Value (0-1).</param>
        /// <returns>An SKColor representing the HSV color.</returns>
        private static SKColor HsvToRgb(float hue, float saturation, float value)
        {
            int hi = (int)(hue / 60) % 6;
            float f = (hue / 60) - hi;
            float p = value * (1 - saturation);
            float q = value * (1 - f * saturation);
            float t = value * (1 - (1 - f) * saturation);
            float r = 0, g = 0, b = 0;
            switch (hi)
            {
                case 0: r = value; g = t; b = p; break;
                case 1: r = q; g = value; b = p; break;
                case 2: r = p; g = value; b = t; break;
                case 3: r = p; g = q; b = value; break;
                case 4: r = t; g = p; b = value; break;
                case 5: r = value; g = p; b = q; break;
            }
            return new SKColor((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }
    }
    public sealed class CanvasRenderEngine
    {
        private Size _size;
        
        // width
        private float _w; 
        private ulong _colWidth;
        // 2xwidth
        private float _2w;
        private float _dt = 5f;
        private float _h;
        private float _dy = 10;
        private ulong _col = 0;
        private SKRect _leftView, _rightView, _view;
        private SortedListDictionary<ushort, object>? _prv;
        private SKBitmap? _bitmap;
        private SKCanvas? _buffer;
        private readonly SKPaint _transparentPaint;
        private readonly ISignalsStream _stream;
        private readonly ConcurrentQueue<Action> _events;
        private SKColor _fontColor;
        private SKColor _pointColor;
        private SKColor _gridLinesStoke;
        private SKColor _axisStroke;
        private SKPaint _fontColorPaint;
        private SKPaint _pointColorPaint;
        private SKPaint _gridLinesStokePaint;
        private SKPaint _axisStrokePaint;
        private DateTime _lastTimestamp;
        private SizeF _gridCellSize = new SizeF(10,50);
        
        public float PointRadius { get; set; } = 5;
        private float _textSize;

        public float TextSize
        {
            get => _textSize;
            set
            {
                if (Math.Abs(value - _textSize) < float.Epsilon) return;
                _textSize = value;
                Invoke(RebuildPaints);
            }
        }

        public SKColor FontColor
        {
            get => _fontColor;
            set
            {
                if (_fontColor == value) return;
                
                _fontColor = value;
                Invoke(RebuildPaints);
            }
        }

        private void RebuildPaints()
        {
            _fontColorPaint?.Dispose();
            _pointColorPaint?.Dispose();
            _gridLinesStokePaint?.Dispose();
            _axisStrokePaint?.Dispose();
            
            _axisStrokePaint = new SKPaint
            {
                Color = _axisStroke, 
                Style = SKPaintStyle.Stroke, 
                StrokeWidth = 2, 
                IsAntialias = true
            };
            
            _fontColorPaint = new SKPaint { 
                Color = _fontColor, 
                TextSize = TextSize,
                Style = SKPaintStyle.Fill, 
                IsAntialias = true };

            _pointColorPaint = new SKPaint { Color = _pointColor, Style = SKPaintStyle.Fill, IsAntialias = true };

            _gridLinesStokePaint = new SKPaint
            {
                Color = _gridLinesStoke, 
                Style = SKPaintStyle.Stroke, 
                StrokeWidth = 1, IsAntialias = true
            };
        }
        public SKColor PointColor
        {
            get => _pointColor;
            set
            {
                if (_pointColor == value) return;

                _pointColor = value;
                Invoke(RebuildPaints);
            }
        }

        public SKColor AxisStroke
        {
            get => _axisStroke;
            set
            {
                if (_axisStroke == value) return;

                _axisStroke = value;
                Invoke(RebuildPaints);
            }

        }
        public SKColor GridLinesStoke
        {
            get => _gridLinesStoke;
            set
            {
                if (_gridLinesStoke == value) return;

                _gridLinesStoke = value;
                Invoke(RebuildPaints);
            }
        }

        public CanvasRenderEngine(ISignalsStream stream, Size? size = null)
        {
            _stream = stream;
            _events = new();
            Size = size ?? new Size(640, 640);
            _transparentPaint = new SKPaint()
            {
                Color = SKColors.Transparent,
                Style = SKPaintStyle.Fill,
                BlendMode = SKBlendMode.Clear
            };
            GridLinesStoke = SKColors.LightGray;
            AxisStroke = SKColors.Black;
            PointColor = SKColors.Green;
            FontColor = SKColors.Black;
            TextSize = 12;
            this._signalSettings = stream.Signals.ToFrozenDictionary(x => x, y => new SignalSettings(this,y));
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
        public void Invoke(Action a)
        {
            // prevent pushing something that is already in the queue.
            if (_events.TryPeek(out var t) && object.ReferenceEquals(t, a))
                return;
            _events.Enqueue(a);
        }

        private void Resize()
        {
            var value = Size;
            _w = value.Width;
            _colWidth = (ulong)(_w / _dt); // celling?
            _2w = _w * 2;
            _h = value.Height;
            _col = 0;
            _buffer?.Dispose();
            _bitmap?.Dispose();
            
            _bitmap = new SKBitmap((int)MathF.Ceiling(_2w), (int)MathF.Ceiling(_h), SKColorType.Bgra8888, SKAlphaType.Opaque);
            _buffer = new SKCanvas(_bitmap);

            _rightView = new SKRect(_w, 0, _2w, _h);
            _view = _leftView = new SKRect(0, 0, _w, _h);
            

            if (Align == Align.Center)
                Offset = new Vector2(0, -_h / 2);
            else if (Align == Align.Bottom)
                Offset = new Vector2(0, 0);
        }
        
        public void Paint(SKPaintSurfaceEventArgs args)
        {
            this.Paint(args.Surface.Canvas);
        }

        private readonly FrozenDictionary<ushort, SignalSettings> _signalSettings;

        public record SignalSettings
        {
            private SKColor _color;

            public SignalSettings(CanvasRenderEngine Parent, ushort id)
            {
                this.Parent = Parent;
                this.Id = id;
                this.Color = ColorGenerator.GetPaintById((byte)id);
            }

            public SKPaint Paint { get; internal set; }

            private SKPaint GetPaint(SKColor value)
            {
                return new SKPaint
                {
                    Color = value,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = Width
                };
            }
            public SKColor Color
            {
                get => _color;
                set
                {
                    if (_color == value) return;
                    _color = value;
                    var prv = Paint;
                    Paint = GetPaint(value);
                    if(prv != null!)
                        Parent.Invoke(() => prv.Dispose());
                }
            }

            public float Width { get; set; } = 2;
            public float Factor { get; set; } = 1;
            public CanvasRenderEngine Parent { get; init; }
            public ushort Id { get; init; }

            public string CssColor
            {
                get { return $"rgba({_color.Red}, {_color.Green}, {_color.Blue}, {_color.Alpha / 255.0f})"; }
            }
        }
        public SignalSettings GetSignalSetting(ushort id)
        {
            if (_signalSettings.TryGetValue(id, out var setting))
                return setting;

            throw new IndexOutOfRangeException();
        }
        
       
        private void DrawSignal(SKCanvas canvas, ushort id, float prvX, object prvValue, float x, object currentValue)
        {
            var factor = GetSignalSetting(id).Factor * _dy;
            
            var prvY = (float)prvValue;
            prvY *= factor;
            var h = _h;
            prvY = h - prvY;
            var currentY = (float)currentValue;
            
            currentY *= factor;
            currentY = h - currentY;

            canvas.DrawLine(prvX, prvY, x, currentY, GetSignalSetting(id).Paint);
            
        }
        
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private bool ShouldDrawTimestamp(DateTime currentTimestamp)
        {
            // we should draw on every second. So we need to find if total seconds have changed since _lastTimestamp, the system can work through a couple of days, so we need to take under consideation day and year
            var totalSeconds = currentTimestamp.ToLongSeconds();
            var lastTotalSeconds = _lastTimestamp.ToLongSeconds();
            bool result = totalSeconds != lastTotalSeconds;
            
            return result;
        }
        public static float CalculateSecondCooridinate(DateTime prv, DateTime n, float x1, float x2)
        {
            // Find the exact time for the next whole second
            DateTime targetSecond = new DateTime(prv.Year, prv.Month, prv.Day, prv.Hour, prv.Minute, prv.Second).AddSeconds(1);

            // Calculate the total time difference between prv and n in milliseconds
            double totalMilliseconds = (n - prv).TotalMilliseconds;

            // Calculate the time difference between prv and the target whole second
            double targetMilliseconds = (targetSecond - prv).TotalMilliseconds;

            // Use proportional interpolation to find the coordinate at the whole second
            // x = x1 + (targetMilliseconds / totalMilliseconds) * (x2 - x1)
            float interpolatedX = x1 + (float)(targetMilliseconds / totalMilliseconds) * (x2 - x1);

            return interpolatedX;
        }
        private void DrawAxisPointBelow(SKCanvas c, 
            float x,
            float y, string text)
        {
            // Draw the circle at the specified x, y coordinates
            c.DrawCircle(x, y, PointRadius, _pointColorPaint);
            // Measure the text width to center it
            var textWidth = _fontColorPaint.MeasureText(text);
            var textX = x - (textWidth / 2);
            // Draw the text below the circle
            c.DrawText(text, textX, y + PointRadius + TextSize*1.2f, _fontColorPaint);
        }
        private void DrawAxisPointRight(SKCanvas c, float x, float y, string text)
        {
            // Draw the circle at the specified x, y coordinates
            c.DrawCircle(x, y, PointRadius, _pointColorPaint);

            // Measure the text height to align it vertically
            var textHeight = _fontColorPaint.TextSize;
            // Draw the text to the right of the circle
            c.DrawText(text, x + PointRadius * 1.5f, y + (textHeight / 2), _fontColorPaint);
        }

        private void DrawGridLineY(SKCanvas c, float x)
        {
            c.DrawLine(x, _h/2, x, _h+_h/2, _gridLinesStokePaint);
        }
        private void DrawHorizontalLines(SKCanvas c, float x, float x2)
        {
            c.DrawLine(x, _h, x2, _h, _axisStrokePaint);
            if (Align == Align.Center)
            {
                for (float dy = _gridCellSize.Height; dy < _h / 2; dy += _gridCellSize.Height)
                {
                    c.DrawLine(x, _h - dy, x2, _h - dy, _gridLinesStokePaint);
                    c.DrawLine(x, _h + dy, x2, _h + dy, _gridLinesStokePaint);
                }
            }
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
                    _prv = result!.Values;
                    _col += 1;
                    _lastTimestamp = result.Timestamp;
                    DrawHorizontalLines(_buffer, 0, _2w);
                    
                    continue;
                }

                bool shouldDrawTimeStamp = ShouldDrawTimestamp(result.Timestamp);
                var prvTimestamp = _lastTimestamp;
                _lastTimestamp = result.Timestamp;
                
                
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
                    
                    
                    _buffer.DrawRect(_leftView, _transparentPaint);
                    _buffer.DrawBitmap(_bitmap, _rightView, _leftView);
                    _buffer.DrawRect(_rightView, _transparentPaint);
                    
                    _buffer!.Save();
                    _buffer.Translate(Offset.X, Offset.Y);
                    DrawHorizontalLines(_buffer, _w, _2w);
                }

                if (x > _w)
                    leftOffset = x - _w;

                if (shouldDrawTimeStamp)
                {
                    var xInterpolated = CalculateSecondCooridinate(prvTimestamp, _lastTimestamp, prvX, x);
                    DrawGridLineY(_buffer, xInterpolated);
                    DrawAxisPointBelow(_buffer, xInterpolated, _h, _lastTimestamp.TimeOfDay.ToString(@"hh\:mm\:ss"));
                }


                //StringBuilder sb = new StringBuilder();
                foreach (var s in result!.Values)
                {
                    if (_prv.TryGetValue(s.Key, out var prvValue))
                    {
                        DrawSignal(_buffer, s.Key, prvX, prvValue, x, s.Value);
                       //sb.Append($"{s.Key}:{s.Value}  ");
                    }
                }
                //Console.WriteLine(sb.ToString());
                _prv.Dispose();
                _prv = result.Values;
                _col += 1;
            }
            
            
            _buffer.Flush();
            _buffer.Restore();

            if (processed == 0) return;
            
            var src = new SKRect(leftOffset, 0, leftOffset + _w, _h);
            
            c.DrawBitmap(_bitmap, src, _view, _bitmapPaint);
            c.DrawTextBox(0, 0, 12, 100, $"{_col}/{processed}", SKColors.LightGray, SKColors.Black);

            // most likely could be painted on layer once. 
            if (Align == Align.Center)
            {
                for (float dy = _gridCellSize.Height; dy < _h / 2; dy += _gridCellSize.Height)
                {
                    var y = -Offset.Y - dy;
                    var y2 = -Offset.Y + dy;
                    c.DrawTextBox(1, y-6,12,25,(dy/_dy).ToString(), SKColors.White, SKColors.Black);
                    c.DrawTextBox(1, y2-6, 12, 30, (-dy / _dy).ToString(), SKColors.White, SKColors.Black);
                }
            }

        }

        private readonly SKPaint _bitmapPaint = new SKPaint() { BlendMode = SKBlendMode.Src };
        private SKRect Rect => _view;

        public SizeF GridCellSize
        {
            get => _gridCellSize;
            set
            {
                if (value == _gridCellSize) return;

                _gridCellSize = value;
            }
        }

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
