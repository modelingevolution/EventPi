// This file is equivalent to Types file in Video Processing.
// We don't share the code, however the values of types for simplicity remains the same for both nugets.

using System.Text.Json.Serialization;
using EventPi.Abstractions;

namespace EventPi.Services.Camera;



public enum VideoTransport : int
{
    Tcp, Udp, Shm
}

public enum VideoCodec : int
{
    Mjpeg, H264, YUV420
}

public enum VideoResolution : int
{
    FullHd, SubHd
}


[JsonConverter(typeof(JsonParsableConverter<Resolution>))]
public readonly record struct Resolution(int Width, int Height) : IParsable<Resolution>
{
    public static readonly Resolution FullHd = new Resolution(1920, 1080);
    public static readonly Resolution SubHd = new Resolution(1456, 1088);
    public override string ToString()
    {
        return $"{Width}x{Height}";
    }

    public static Resolution Parse(string s, IFormatProvider? provider)
    {
        var segments = s.Split('x');
        return new Resolution(int.Parse(segments[0]), int.Parse(segments[1]));
    }

    public static bool TryParse(string? s, out Resolution result) =>
        Resolution.TryParse(s, null, out result);

    public static bool TryParse(string? s, IFormatProvider? provider, out Resolution result)
    {
        if(s == null)         {
            result = default;
            return false;
        }
        var segments = s.Split('x');
        if(segments.Length != 2)
        {
            result = default;
            return false;
        }
        if(int.TryParse(segments[0], out var w) && int.TryParse(segments[1], out var h))
        {
            result = new Resolution(w, h);
            return true;
        }
        result = default;
        return false;
    }
}