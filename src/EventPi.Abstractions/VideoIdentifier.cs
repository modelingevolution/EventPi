using ProtoBuf;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Grpc.Core;
using ProtoBuf.Meta;

namespace EventPi.Abstractions;

public interface IVideoRecodingLocator
{
    bool Exists(in VideoRecordingIdentifier recording);
    bool Exists(in FrameId frameId);
    RecordingPath GetPath(in VideoRecordingIdentifier recording);
    string GetFolderFullPath(in VideoRecordingIdentifier recording);
    IEnumerable<VideoRecordingIdentifier> Recording();
}

public readonly record struct RecordingPath(string DataPath, string IndexPath);

[ProtoContract]
public class DateTimeOffsetSurrogate
{
    [ProtoMember(1)]
    public long Ticks { get; set; }

    [ProtoMember(2)]
    public TimeSpan Offset { get; set; }

    public static implicit operator DateTimeOffsetSurrogate(DateTimeOffset value)
        => new DateTimeOffsetSurrogate
        {
            Ticks = value.Ticks,
            Offset = value.Offset
        };

    public static implicit operator DateTimeOffset(DateTimeOffsetSurrogate value)
        => new DateTimeOffset(value.Ticks, value.Offset);
}
public static class ProtobufConfig
{
    private static bool _configured = false;
    public static void Configure()
    {
        if(!_configured)
            RuntimeTypeModel.Default
            .Add(typeof(DateTimeOffset), false)
            .SetSurrogate(typeof(DateTimeOffsetSurrogate));
        _configured = true;
    }
}


[JsonConverter(typeof(JsonParsableConverter<VideoRecordingIdentifier>))]
[ProtoContract]
public readonly record struct VideoRecordingIdentifier : IParsable<VideoRecordingIdentifier>
{
    [ProtoMember(1)]
    public HostName HostName { get; init;  }

    [ProtoMember(2)]
    public int? CameraNumber { get; init; }

    [ProtoMember(3)]
    public DateTimeOffset CreatedTime { get; init; }

    public VideoRecordingIdentifier()
    {
        
    }

    public VideoRecordingIdentifier(HostName hostName, DateTimeOffset createdTime)
    {
        HostName = hostName;
        CameraNumber = null;
        CreatedTime = createdTime;
    }

    public VideoRecordingIdentifier(HostName hostName, int cameraNumber, DateTimeOffset createdTime)
    {
        HostName = hostName;
        CameraNumber = cameraNumber;
        CreatedTime = createdTime;
    }

    public static VideoRecordingIdentifier Parse(string s, IFormatProvider? provider)
    {
        if (string.IsNullOrEmpty(s)) throw new ArgumentNullException(nameof(s));

        var parts = s.Split('/');
        if (parts.Length != 2) throw new FormatException("Invalid format for VideoSourceIdentifier.");

        var sourceInfo = parts[0].Split(':');
        if (sourceInfo.Length == 1)
        {
            var hostName = HostName.Parse(sourceInfo[0], provider);
            var createdTime = DateTime.ParseExact(parts[1], "o", provider);
            return new VideoRecordingIdentifier(hostName, createdTime);
        }
        else
        {
            var hostName = HostName.Parse(sourceInfo[0], provider);
            var cameraId = int.Parse(sourceInfo[1]);
            var createdTime = DateTime.ParseExact(parts[1], "o", provider);

            return new VideoRecordingIdentifier(hostName, cameraId, createdTime);
        }
    }
    public string ToStringFileName()
    {
        // Convert to filename-safe ISO 8601
        if (CreatedTime.Offset == TimeSpan.Zero)
        {
            // Use Z for UTC
            var utcStr = CreatedTime.UtcDateTime.ToString("yyyyMMddTHHmmss.ffffff") + "Z";
            return CameraNumber.HasValue && CameraNumber.Value != 0
                ? $"{HostName}.{CameraNumber}.{utcStr}"
                : $"{HostName}.{utcStr}";
        }
        else
        {
            // Use numeric offset (e.g., +0100, -0500)
            var dateStr = CreatedTime.ToString("yyyyMMddTHHmmss.ffffff");
            var offsetStr = CreatedTime.ToString("zzz").Replace(":", "");
            var fullDateStr = dateStr + offsetStr;

            return CameraNumber.HasValue && CameraNumber.Value != 0
                ? $"{HostName}.{CameraNumber}.{fullDateStr}"
                : $"{HostName}.{fullDateStr}";
        }
    }

    public static bool TryParseFileName(string fileName, out VideoRecordingIdentifier result)
    {
        result = default;
        if (string.IsNullOrEmpty(fileName)) return false;

        // Split the filename parts by dots
        var parts = fileName.Split('.');
        if (parts.Length < 3 || parts.Length > 4) return false;

        // Parse hostname (first part)
        if (!HostName.TryParse(parts[0], null, out var hostName)) return false;

        // Parse the datetime part (last part)
        string dateTimePart = $"{parts[^2]}.{parts[^1]}";
        if (dateTimePart.Length < 20) return false; // Basic length validation

        try
        {
            // Handle both Z and offset formats
            DateTimeOffset parsedTime;
            if (dateTimePart.EndsWith("Z"))
            {
                // UTC format
                
                var utcDateTime = DateTime.ParseExact(
                    dateTimePart.TrimEnd('Z'),
                    "yyyyMMddTHHmmss.ffffff",
                    null,
                    System.Globalization.DateTimeStyles.AssumeUniversal |
                    System.Globalization.DateTimeStyles.AdjustToUniversal);
                parsedTime = new DateTimeOffset(utcDateTime, TimeSpan.Zero);
            }
            else
            {
                // With offset format: split into datetime and offset parts
                int signPos = dateTimePart.LastIndexOfAny(new[] { '+', '-' });
                if (signPos == -1) return false;

                string dtPart = dateTimePart.Substring(0, signPos);
                string offsetPart = dateTimePart.Substring(signPos);

                // Parse the date time
                var dt = DateTime.ParseExact(
                    dtPart,
                    "yyyyMMddTHHmmss.ffffff",
                    null,
                    System.Globalization.DateTimeStyles.None);

                // Parse the offset (+0100 or -0500 format)
                if (offsetPart.Length != 5) return false; // +/- plus 4 digits

                int offsetHours = int.Parse(offsetPart.Substring(1, 2));
                int offsetMinutes = int.Parse(offsetPart.Substring(3, 2));
                var offset = new TimeSpan(offsetHours, offsetMinutes, 0);
                if (offsetPart[0] == '-') offset = -offset;

                parsedTime = new DateTimeOffset(dt, offset);
            }

            // Create result based on whether we have a camera number
            if (parts.Length == 4)
            {
                if (!int.TryParse(parts[1], out int cameraNumber)) return false;
                result = new VideoRecordingIdentifier(hostName, cameraNumber, parsedTime);
            }
            else
            {
                result = new VideoRecordingIdentifier(hostName, parsedTime);
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out VideoRecordingIdentifier result)
    {
        result = default;
        if (string.IsNullOrEmpty(s)) return false;

        var parts = s.Split('/');
        if (parts.Length != 2) return false;

        var sourceInfo = parts[0].Split(':');
        if (sourceInfo.Length != 2) return false;

        if (!HostName.TryParse(sourceInfo[0], provider, out var hostName)) return false;
        if (!int.TryParse(sourceInfo[1], out var cameraId)) return false;
        if (!DateTime.TryParseExact(parts[1], "o", provider, System.Globalization.DateTimeStyles.None, out var createdTime))
            return false;

        result = new VideoRecordingIdentifier(hostName, cameraId, createdTime);
        return true;
    }

    public override string ToString()
    {
        return CameraNumber.HasValue ? $"{HostName}:{CameraNumber}/{CreatedTime:o}" : $"{HostName}/{CreatedTime:o}";
    }

    public static implicit operator Guid(VideoRecordingIdentifier addr)
    {
        return addr.ToString().ToGuid();
    }

    public static implicit operator VideoRecordingDevice(VideoRecordingIdentifier addr)
    {
        return new VideoRecordingDevice { HostName = addr.HostName, CameraNumber = addr.CameraNumber };
    }

    public static implicit operator VideoRecordingIdentifier(VideoAddress addr)
    {
        var hostName = HostName.Parse(addr.Host);
        return addr.VideoSource == VideoSource.File
            ? new VideoRecordingIdentifier(hostName, DateTimeOffset.Now)
            : new VideoRecordingIdentifier(hostName, addr.CameraNumber ?? 0, DateTimeOffset.Now);
    }

    public static implicit operator VideoRecordingIdentifier(CameraAddress addr)
    {
        return new VideoRecordingIdentifier(addr.HostName, addr.CameraNumber ?? 0, DateTimeOffset.Now);
    }
}