using System.Drawing;

namespace EventPi.Abstractions
{
    public record struct PanConfig(int Size, Point Center) : IParsable<PanConfig>
    {
        public PanConfig(int size, int cx, int cy) : this(size, new Point(cx, cy))
        {

        }
        public static PanConfig Parse(string s, IFormatProvider? provider = null)
        {
            if (string.IsNullOrEmpty(s)) throw new ArgumentNullException(nameof(s));

            var parts = s.Split('_');
            if (parts.Length != 3) throw new FormatException("Invalid format for PanConfig.");

            var size = int.Parse(parts[0]);
            var cx = int.Parse(parts[1]);
            var cy = int.Parse(parts[2]);

            return new PanConfig(size, cx, cy);

        }
        public static implicit operator Guid(PanConfig pan)
        {
            return pan.ToString().ToGuid();
        }
        public static bool TryParse(string? input, IFormatProvider? formatProvider, out PanConfig result)
        {
            try
            {
                result = Parse(input!, formatProvider);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }
        public override string ToString()
        {
            return $"{Size}_{Center.X}_{Center.Y}";
        }
    }
}
