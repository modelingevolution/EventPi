using System.Text.Json.Serialization;

namespace EventPi.Abstractions;

[JsonConverter(typeof(JsonParsableConverter<CameraAddress>))]
public readonly struct CameraAddress : IParsable<CameraAddress>, IEquatable<CameraAddress>
{
    public HostName HostName { get; init; }
    public int CameraNumber { get; init;  }


    public static CameraAddress Parse(string s, IFormatProvider? provider)
    {
        int b = s.LastIndexOf('/');
        if (b == -1)
            return new CameraAddress { HostName = HostName.Parse(s) };
        string h = s.Substring(0, b);
        int nr = int.Parse(s.Substring(b + 1));
        return new CameraAddress { HostName = HostName.Parse(h), CameraNumber = nr };
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out CameraAddress result)
    {
        int b = s.LastIndexOf('/');
        if (b == -1)
        {
            result = new CameraAddress { HostName = HostName.Parse(s) };
            return true;
        }
        string h = s.Substring(0, b);
        if (int.TryParse(s.Substring(b + 1), out var c))
        { 
            result = new CameraAddress { HostName = HostName.Parse(s), CameraNumber = c };
            return true;
        }
        result = new CameraAddress { HostName = HostName.Parse(s) };
        return false;
    }

    public override bool Equals(object? obj)
    {
        return obj is CameraAddress address &&
               this.Equals(address);
    }

    public bool Equals(CameraAddress address)
    {
        return HostName.Equals(address.HostName) &&
               CameraNumber == address.CameraNumber;
    }

    public static bool operator ==(CameraAddress left, CameraAddress right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CameraAddress left, CameraAddress right)
    {
        return !(left == right);
    }
}
