using EventPi.Abstractions;
using Tmds.DBus.Protocol;

namespace EventPi.NetworkMonitor;

#pragma warning disable CS1998
public readonly struct PathId : IEquatable<PathId>, IComparable<PathId>, IComparable
{
    private readonly Guid _id;
    private readonly string? _path;

    private PathId(Guid id)
    {
        _id = id;
        _path = null;
    }

    private PathId(string path)
    {
        _path = path;
        _id = _path.ToGuid();
    }
    public string Path => _path;
    public Guid Id => _id;

    public static implicit operator PathId(string path) => new PathId(path);
    public static implicit operator PathId(Guid id) => new PathId(id);
    public static implicit operator ObjectPath(PathId p) => p.Path;
    public static implicit operator PathId(ObjectPath p) => new PathId(p);

    public bool Equals(PathId other) => _id.Equals(other._id);

    public override bool Equals(object? obj) => obj is PathId other && Equals(other);

    public override int GetHashCode() => _id.GetHashCode();

    public static bool operator ==(PathId left, PathId right) => left.Equals(right);

    public static bool operator !=(PathId left, PathId right) => !left.Equals(right);

    public int CompareTo(PathId other) => _id.CompareTo(other._id);

    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        return obj is PathId other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(PathId)}");
    }

    public override string ToString() => _path ?? _id.ToString();
}