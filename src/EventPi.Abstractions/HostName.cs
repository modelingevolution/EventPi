using System.Text.Json.Serialization;

namespace EventPi.Abstractions;

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