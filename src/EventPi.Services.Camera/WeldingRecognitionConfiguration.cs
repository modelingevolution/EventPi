using EventPi.Abstractions;
using MicroPlumberd;

namespace EventPi.Services.Camera;

[OutputStream("WeldingRecognitionConfiguration")]
public record WeldingRecognitionConfiguration : IStatefulStream<HostName>
{
    public static string FullStreamName(HostName id) => $"WeldingRecognitionConfiguration-{id}";
}