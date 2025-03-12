using System.Text.Json.Serialization;
using MicroPlumberd;

namespace EventPi.Abstractions;

[JsonConverter(typeof(JsonParsableConverter<Ip4>))]
public readonly record struct Ip4 : IParsable<Ip4>
{
    private readonly uint _value;
    public static Ip4 Loopback { get; } = "127.0.0.1";
    

    public Ip4 Network(uint prefix)
    {
        uint mask = ~((1u << (32 - (int)prefix)) - 1);
        return new Ip4(mask & _value);
    }
    public override string ToString()
    {
        byte[] bytes = BitConverter.GetBytes(_value);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return $"{bytes[0]}.{bytes[1]}.{bytes[2]}.{bytes[3]}";
    }

    public static implicit operator Ip4(string v)
    {
        return Ip4.Parse(v, null);
    }
    public static implicit operator Ip4(uint v)
    {
        return new Ip4(v);
    }
    private Ip4(uint value)
    {
        _value = value;}
    public static Ip4 Parse(string s, IFormatProvider? provider)
    {
        return TryParse(s, provider, out var i) ? i : new Ip4(0u);
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out Ip4 result)
    {
        // Check for null input
        if (string.IsNullOrEmpty(s))
        {
            result = default;
            return false;
        }

        string[] parts = s.Split('.');
        if (parts.Length == 4)
        {
            byte[] bytes = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                if (byte.TryParse(parts[i], out bytes[i]) && bytes[i] >= 0 && bytes[i] <= 255)
                    continue;

                result = default;
                return false;
            }

            // Consider byte order for network representation (big-endian)
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            result = new Ip4(BitConverter.ToUInt32(bytes));
            return true;
        }

        result = default;
        return false;
    }
}