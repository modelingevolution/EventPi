using System.Text.Json.Serialization;

namespace EventPi.Abstractions;

[JsonConverter(typeof(JsonParsableConverter<Ip4>))]
public readonly record struct Ip4 : IParsable<Ip4>
{
    private readonly uint _value;
    public static Ip4 Loopback { get; } = "127.0.0.1";

    public override string ToString()
    {
        byte[] bytes = BitConverter.GetBytes(_value);
        return string.Join('.', bytes);
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
        string[] parts = s.Split('.');
        if (parts.Length == 4)
        {
            byte[] bytes = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                if (byte.TryParse(parts[i], out bytes[i])) continue;
                result = default;
                return false;
            }
            result = new Ip4(BitConverter.ToUInt32(bytes));
            return true;
        }
        result = default;
        return false;
    }
}