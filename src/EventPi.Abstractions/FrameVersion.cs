using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPi.Abstractions
{
    // ToString = T:Version, we search for last ':' char in the string.
    public readonly record struct Reference<T> : IParsable<Reference<T>>
        where T : struct, IParsable<T>
    {
        
       
        public uint Version { get; init; }
        public T Value { get; init; }
        public static Reference<T> Parse(string s, IFormatProvider? provider)
        {
            if (string.IsNullOrEmpty(s)) throw new ArgumentNullException(nameof(s));

            var lastColonIndex = s.LastIndexOf(':');
            if (lastColonIndex < 0 || lastColonIndex == s.Length - 1) throw new FormatException("Input string is not in the correct format.");

            var valuePart = s.Substring(0, lastColonIndex);
            var versionPart = s.Substring(lastColonIndex + 1);

            if (!uint.TryParse(versionPart, out var version)) throw new FormatException("Version part is not a valid unsigned integer.");

            var value = T.Parse(valuePart, provider);

            return new Reference<T> { Value = value, Version = version };
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Reference<T> result)
        {
            result = default;

            if (string.IsNullOrEmpty(s)) return false;

            var lastColonIndex = s.LastIndexOf(':');
            if (lastColonIndex < 0 || lastColonIndex == s.Length - 1) return false;

            var valuePart = s.Substring(0, lastColonIndex);
            var versionPart = s.Substring(lastColonIndex + 1);

            if (!uint.TryParse(versionPart, out var version)) return false;

            if (!T.TryParse(valuePart, provider, out var value)) return false;

            result = new Reference<T> { Value = value, Version = version };
            return true;
        }

        public override string ToString()
        {
            return $"{Value}:{Version}";
        }
    }
}
