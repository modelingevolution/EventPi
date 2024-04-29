using System.ComponentModel.DataAnnotations;
using MicroPlumberd;

namespace EventPi.Services.Camera.Cli;

[OutputStream("Camera")]
public class SetCameraWeldingParams
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Range(0, 400*400)]
    public int WeldingBorder { get; set; }
    [Range(0, 400*400)]
    public int NonWeldingBorder { get; set; }
    [Range(1, 50)]
    public int WeldingDataBufferSize { get; set; }
}