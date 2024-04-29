using MicroPlumberd;
using MicroPlumberd.Services;

namespace EventPi.Services.Camera.Cli;

[CommandHandler]
public partial class CameraWeldingBorderConfigurationCommandHandler(IPlumber plumber)
{
    public async Task Handle(Guid id, SetCameraWeldingParams cmd)
    {
        var ev = new CameraWeldingParamsDefined()
        {
            WeldingBorder = cmd.WeldingBorder,
            NonWeldingBorder = cmd.NonWeldingBorder,
            WeldingDataBufferSize = cmd.WeldingDataBufferSize
        };

        await plumber.AppendEvent(ev);
    }



}