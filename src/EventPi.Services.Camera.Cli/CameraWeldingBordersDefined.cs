using MicroPlumberd;

namespace EventPi.Services.Camera.Cli;

[OutputStream("CameraConfiguration")]
public record CameraWeldingParamsDefined
{
    public int WeldingBorder { get; set; }
    public int NonWeldingBorder { get; set; }
    public int WeldingDataBufferSize { get; set; }
   
}