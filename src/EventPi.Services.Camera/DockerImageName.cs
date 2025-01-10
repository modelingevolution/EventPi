using System.Diagnostics.CodeAnalysis;

namespace EventPi.Services.Camera;

public readonly record struct DockerImageName : IParsable<DockerImageName>
{
    public string Name { get; init; }
    public string Tag { get; init; }
    public DockerImageName(string name, string? tag = null) => (Name, Tag) = (name, tag ?? "latest");
    
    public static implicit operator DockerImageName(string s) => Parse(s, null);
    
    public static DockerImageName Parse(string s, IFormatProvider? provider)
    {
        if (string.IsNullOrWhiteSpace(s)) throw new ArgumentException("Input string cannot be null or whitespace.", nameof(s));

        var parts = s.Split(':');
        if (parts.Length != 2) throw new FormatException("Input string must be in the format 'name:tag'.");

        return new DockerImageName(parts[0], parts[1]);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider,
        [MaybeNullWhen(false)] out DockerImageName result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(s)) return false;

        var parts = s.Split(':');
        if (parts.Length != 2) return false;

        result = new DockerImageName(parts[0], parts[1]);
        return true;
    }
    
    public override string ToString() => $"{Name}:{Tag}";
}