using ProtoBuf;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventPi.Abstractions;

[ProtoContract]
[JsonConverter(typeof(JsonParsableConverter<FrameId>))]
public readonly struct FrameId : IEquatable<FrameId>, IComparable<FrameId>,
    IComparable, IParsable<FrameId>
{
    private readonly ulong _frameNumber;
    
    [ProtoMember(0)]
    public VideoRecordingIdentifier Recording { get; init; }



    public ulong FrameNumber { get => _frameNumber; private init => _frameNumber=value; }


    public FrameId(VideoRecordingIdentifier recording, ulong frameNumber)
    {
        Recording = recording;
        _frameNumber = frameNumber;
    }

    public static FrameId From(VideoRecordingIdentifier identifier, ulong frameNumber)
    {
        return new FrameId(identifier, frameNumber);
    }

    public static implicit operator Guid(FrameId frame)
    {
        return frame.ToString().ToGuid();
    }
    public static implicit operator string(FrameId frameId) => frameId.ToString();
    public static FrameId From(string frameId)
    {
        return Parse(frameId);
    }
    public bool Equals(FrameId other)
    {
        return this.Recording.Equals(other.Recording) && (this._frameNumber == other._frameNumber);
    }

    public override bool Equals(object obj) => obj is FrameId other && Equals(other);

    public override int GetHashCode()
    {
        return (ToString() != null ? ToString().GetHashCode(StringComparison.OrdinalIgnoreCase) : 0);
    }
    
    public override string ToString() => $"{Recording}/{FrameNumber}";

    public static bool operator ==(FrameId left, FrameId right) => left.Equals(right);

    public static bool operator !=(FrameId left, FrameId right) => !left.Equals(right);

    public int CompareTo(FrameId other)
    {
        return string.Compare(ToString(), other.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    public int CompareTo(object obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        return obj is FrameId other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(FrameId)}");
    }

    public static FrameId Parse(string input, IFormatProvider? formatProvider = null)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentNullException(nameof(input), "Input string cannot be null or empty.");
        }

        try
        {
            int lastIndex = input.LastIndexOf('/');
            string recording = input.Remove(lastIndex);
            string number = input.Substring(lastIndex + 1);
            return new FrameId(VideoRecordingIdentifier.Parse(recording, null), ulong.Parse(number));
        }
        catch (Exception ex) when (ex is not FormatException)
        {
            throw new FormatException("Failed to parse input string", ex);
        }
    }
    public static bool TryParse(string? input, IFormatProvider? formatProvider, out FrameId result)
    {
        try
        {
            result = Parse(input!, formatProvider);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}
