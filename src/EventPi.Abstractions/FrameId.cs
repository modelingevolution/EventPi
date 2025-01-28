using ProtoBuf;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using EventStore.Client;

namespace EventPi.Abstractions;

[ProtoContract]
[JsonConverter(typeof(JsonParsableConverter<FrameId>))]
public readonly record struct FrameId : IEquatable<FrameId>, IComparable<FrameId>,
    IComparable, IParsable<FrameId>
{
    [ProtoMember(1)]
    public VideoRecordingIdentifier Recording { get; init; }


    [ProtoMember(2)]
    public ulong FrameNumber { get; init; }

    public static readonly FrameId Empty = new FrameId();
    public FrameId()
    {
        
    }
    public FrameId(VideoRecordingIdentifier recording, ulong frameNumber)
    {
        Recording = recording;
        FrameNumber = frameNumber;
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
    public override string ToString() => $"{Recording}/{FrameNumber}";
    public string ToStringFileName()
    {
        string recordingPart = Recording.ToStringFileName();
        return $"{recordingPart}.{FrameNumber}";
    }


    public int CompareTo(FrameId other)
    {
        return string.Compare(ToString(), other.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    public int CompareTo(object obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        return obj is FrameId other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(FrameId)}");
    }
    public static bool TryParseFileName(string fileName, out FrameId result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(fileName))
            return false;

        int lastDotIndex = fileName.LastIndexOf('.');
        if (lastDotIndex == -1)
            return false;

        string recordingPart = fileName[..lastDotIndex];
        string frameNumberPart = fileName[(lastDotIndex + 1)..];

        if (!VideoRecordingIdentifier.TryParseFileName(recordingPart, out var recording))
            return false;

        if (!ulong.TryParse(frameNumberPart, out var frameNumber))
            return false;

        result = new FrameId(recording, frameNumber);
        return true;
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
