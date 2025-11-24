using System.Text.Json.Serialization;
using EventPi.Abstractions;
using ModelingEvolution.JsonParsableConverter;

namespace EventPi.Services.Camera.Contract;

[JsonConverter(typeof(JsonConverter<JsonParsableConverter<HostProfilePath>>))]
public readonly record struct HostProfilePath : IParsable<HostProfilePath>
{
    public static readonly HostProfilePath Empty = new HostProfilePath();
    public HostName HostName { get; init; }
    public string ProfileName { get; init; }
    public override string ToString()
    {
        return $"{HostName}/{ProfileName}";
    }

    public static HostProfilePath Create(string hostName, string profile)
    {
        if (string.IsNullOrWhiteSpace(hostName)) throw new ArgumentException(nameof(hostName));
        if (string.IsNullOrWhiteSpace(profile)) throw new ArgumentException(nameof(profile));
        return new HostProfilePath() { HostName = (HostName)hostName, ProfileName = profile };
    }
    public static HostProfilePath Parse(string s, IFormatProvider? provider)
    {
        var index = s.IndexOf('/');
        string hostName = s.Remove(index);
        string profile = s.Substring(index + 1);
        return new HostProfilePath() { HostName = (HostName)hostName, ProfileName = profile };
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out HostProfilePath result)
    {
        result = Empty;
        if (s == null) return false;
        var index = s.IndexOf('/');
        if(index == -1) return false;

        string hostName = s.Remove(index);
        string profile = s.Substring(index + 1);
        

        result = new HostProfilePath() { HostName = (HostName)hostName, ProfileName = profile };
        return true;

    }
}