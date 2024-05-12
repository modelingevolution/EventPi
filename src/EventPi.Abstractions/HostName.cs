using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace EventPi.Abstractions;

public static class Extensions
{
    public static byte[] ToHash(this string t)
    {
        using (SHA256 shA256 = SHA256.Create())
        {
            byte[] hash = shA256.ComputeHash(Encoding.Default.GetBytes(t));
            ulong uint64_1 = BitConverter.ToUInt64(hash, 0);
            ulong uint64_2 = BitConverter.ToUInt64(hash, 8);
            ulong uint64_3 = BitConverter.ToUInt64(hash, 16);
            ulong uint64_4 = BitConverter.ToUInt64(hash, 24);
            ulong num1 = uint64_1 ^ uint64_3;
            ulong num2 = uint64_2 ^ uint64_4;
            Memory<byte> memory = new Memory<byte>(new byte[16]);
            BitConverter.TryWriteBytes(memory.Span, num1);
            BitConverter.TryWriteBytes(memory.Slice(8, 8).Span, num2);
            return memory.ToArray();
        }
    }

    public static Guid ToGuid(this string t) => new Guid(t.ToHash());


}

[JsonConverter(typeof(JsonParsableConverter<HostName>))]
public readonly struct HostName : IEquatable<HostName>, IComparable<HostName>, IComparable, IParsable<HostName>
{
    public static readonly HostName Empty = new HostName(string.Empty);
    public static readonly HostName Localhost = new HostName("localhost");
    private readonly string _name;

    private HostName(string n) => _name = n;
    public static explicit operator HostName(string hostname) => new HostName(hostname);
    public static implicit operator string(HostName hostname) => hostname.ToString();
    public static HostName From(string name) => new HostName(name);
    public bool Equals(HostName other)
    {
        return string.Equals(this._name, other._name, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object obj) => obj is HostName other && Equals(other);

    public override int GetHashCode()
    {
        return (_name != null ? _name.GetHashCode(StringComparison.OrdinalIgnoreCase) : 0);
    }

    public override string ToString() => _name;

    public static bool operator ==(HostName left, HostName right) => left.Equals(right);

    public static bool operator !=(HostName left, HostName right) => !left.Equals(right);

    public int CompareTo(HostName other)
    {
        return string.Compare(_name, other._name, StringComparison.OrdinalIgnoreCase);
    }

    public int CompareTo(object obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        return obj is HostName other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(HostName)}");
    }

    public static HostName Parse(string s, IFormatProvider? provider)
    {
        return HostName.From(s);
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out HostName result)
    {
        result = HostName.From(s);
        return true;
    }
}