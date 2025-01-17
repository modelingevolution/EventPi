using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventPi.Abstractions;

[JsonConverter(typeof(JsonParsableConverter<FrameId>))]
public readonly struct FrameId : IEquatable<FrameId>, IComparable<FrameId>,
    IComparable, IParsable<FrameId>
{

    public HostName Device { get => _device; private init
        {
            _device=value;
        }
    }
    public long FrameNumber { get => _frameNumber; private init
        {
            _frameNumber=value;
        }
    }
    public int CameraId { get => _cameraId; private init
        {
            _cameraId=value;
        }
    }
    public DateTimeOffset RecordingDate { get => _recordingDate; private init
        {
            _recordingDate=value;
        }
    }

    private readonly HostName _device;
    private readonly long _frameNumber;
    private readonly int _cameraId;
    private readonly DateTimeOffset _recordingDate;

    private FrameId(HostName device, long frameNumber, int cameraId, DateTimeOffset recordingDate)
    {
        _device = device;
        _frameNumber = frameNumber;
        _recordingDate = recordingDate;
        _cameraId = cameraId;
    }

    public static implicit operator string(FrameId frameId) => frameId.ToString();
    public static FrameId From(string frameId)
    {
        return Parse(frameId);
    }
    public bool Equals(FrameId other)
    {
        return HostName.Equals(this._device, other._device) && (this._frameNumber == other._frameNumber) &&(this._cameraId == other._cameraId) &&this._recordingDate.Equals(other._recordingDate);
    }

    public override bool Equals(object obj) => obj is FrameId other && Equals(other);

    public override int GetHashCode()
    {
        return (ToString() != null ? ToString().GetHashCode(StringComparison.OrdinalIgnoreCase) : 0);
    }

    public override string ToString() =>
    $"frame://{_device.ToString()}:{_cameraId}/{_recordingDate.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)}/{_frameNumber.ToString()}";

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

        var url = new Uri(input);
        const string prefix = "frame";
        if (!url.Scheme.Equals(prefix, StringComparison.Ordinal))
        {
            throw new FormatException($"Input string must start with '{prefix}'");
        }

        try
        {
            // Remove the prefix and split the remaining string
            string[] segments = url.Segments;

            if (segments.Length != 3)
            {
                throw new FormatException("URI format must contain exactly 4 segments: device/cameraId/recordingDate/frameNumber");
            }

            if (!DateTimeOffset.TryParse(segments[1].Replace("/",""), formatProvider, DateTimeStyles.None, out DateTimeOffset recordingDate))
            {
                throw new FormatException($"Invalid date format: {segments[1]}");
            }

            if (!long.TryParse(segments[2], NumberStyles.Integer, formatProvider, out long frameNumber))
            {
                throw new FormatException($"Invalid frame number format: {segments[2]}");
            }

            return new FrameId
            {
                Device = HostName.From(url.Host),
                CameraId = url.Port,
                RecordingDate = recordingDate,
                FrameNumber = frameNumber
            };
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
