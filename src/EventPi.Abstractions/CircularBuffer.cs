using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;
using MicroPlumberd;


namespace EventPi.Abstractions;

public interface IStatefulStream<in TIdentifier>
{
    static abstract string FullStreamName(TIdentifier id);
}

public static class MetadataExtensions
{
    public static T StreamId<T>(this Metadata m) where T:IParsable<T>
    {
        int index = m.SourceStreamId.IndexOf('-');
        string id = m.SourceStreamId.Substring(index + 1);
        return T.Parse(id,null);
    }
}
public class JsonParsableConverter<T> : JsonConverter<T> where T:IParsable<T>
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();
        return T.Parse(str, null);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
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
public class CircularBuffer<T> : IEnumerable<T>
{
    private readonly T[] _buffer;
    private int _head;
    private int _tail;
    private int _count;

    public int Capacity { get; private set; }

    public CircularBuffer(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentException("Circular buffer capacity must be positive", nameof(capacity));
        }

        Capacity = capacity;
        _buffer = new T[Capacity];
        _head = 0;
        _tail = 0;
        _count = 0;
    }

    public T Head()
    {
        return _buffer[_head];
    }
    public bool AddLast(T item)
    {

        bool ret = false;
        // If the buffer is full, move the head to the next oldest element
        if (_count == Capacity)
        {

            _head = (_head + 1) % Capacity;
            ret = true;
        }
        else
            _count++;

        // Add the new item at the tail and move the tail to the next position
        _buffer[_tail] = item;
        _tail = (_tail + 1) % Capacity;
        return ret;
    }
    public T? Remove()
    {
        if (_count == 0)
            return default(T);

        T item = _buffer[_head];
        _head = (_head + 1) % Capacity;
        _count--;
        return item;
    }
    public IEnumerator<T> GetEnumerator()
    {
        var h = _head;
        var c = _count;
        for (int i = 0; i < c; i++)
        {
            yield return _buffer[(h + i) % Capacity];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}