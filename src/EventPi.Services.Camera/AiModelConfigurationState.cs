using System.Drawing;
using System.Text.Json.Serialization;
using EventPi.Abstractions;
using MicroPlumberd;

namespace EventPi.Services.Camera;

[OutputStream("AiModelConfiguration")]
public record AiModelConfigurationState
{
    public static string StreamName(HostName host, int cameraNr)
    {
        string streamName = $"AiModelConfiguration-{host}/{cameraNr}";
        return streamName;
    }
    
    [JsonConverter(typeof(JsonRectangleConverter))]
    public Rectangle InterestRegion { get; init; }

    public AiModelConfigurationState()
    {
        InterestRegion = new Rectangle(1920/2-640/2,1080/2-640/2,640,640);
    }
}