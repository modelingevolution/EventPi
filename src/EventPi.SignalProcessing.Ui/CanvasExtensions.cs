using System.Text;
using SkiaSharp;

namespace EventPi.SignalProcessing.Ui
{
    public static class CanvasExtensions
    {
        public static void DrawTextBox(this SKCanvas canvas, float x, float y, float fontSize,
        float width,
        string text, SKColor bg, SKColor fontColor)
        {
            using var backgroundPaint = new SKPaint
            {
                Color = bg,
                Style = SKPaintStyle.Fill
            };

            using var textPaint = new SKPaint
            {
                Color = fontColor,
                TextSize = fontSize,
                IsAntialias = true,
                Typeface = SKTypeface.Default
            };

            // Measure the text to determine height and line wrapping
            var textBounds = new SKRect();
            textPaint.MeasureText(text, ref textBounds);

            // Calculate text width and height with word wrapping
            var paragraphWidth = width;
            var textWidth = textPaint.MeasureText(text);
            var textHeight = textBounds.Height;

            // Determine if text needs to be wrapped
            var lines = new List<string>();
            if (textWidth > paragraphWidth)
            {
                // Word wrap logic
                var words = text.Split(' ');
                var currentLine = new StringBuilder();
                foreach (var word in words)
                {
                    var testLine = currentLine.Length == 0
                        ? word
                        : $"{currentLine} {word}";

                    if (textPaint.MeasureText(testLine) <= paragraphWidth)
                    {
                        currentLine.Append(currentLine.Length == 0 ? word : $" {word}");
                    }
                    else
                    {
                        lines.Add(currentLine.ToString());
                        currentLine.Clear();
                        currentLine.Append(word);
                    }
                }

                if (currentLine.Length > 0)
                    lines.Add(currentLine.ToString());
            }
            else
            {
                lines.Add(text);
            }

            // Calculate total text box height
            var totalTextHeight = lines.Count * textBounds.Height * 1.2f; // 1.2 for line spacing

            // Draw background rectangle
            var backgroundRect = new SKRect(x, y, x + width, y + totalTextHeight + 10);
            canvas.DrawRect(backgroundRect, backgroundPaint);

            // Draw text
            float currentY = y + textBounds.Height * 1.2f;
            foreach (var line in lines)
            {
                canvas.DrawText(line, x + 5, currentY, textPaint);
                currentY += textBounds.Height * 1.2f;
            }
        }
    }
}
