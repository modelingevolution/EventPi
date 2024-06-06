using EventPi.Abstractions;
using EventPi.Services.Camera.Contract;
using MicroPlumberd;
using MicroPlumberd.Services;

namespace EventPi.Services.Camera;

[CommandHandler]
public partial class WeldingRecognitionCommandHandler(IPlumber plumber, GrpcCppCameraProxy proxy)
{
    public async Task Handle(HostName hostName, SetWeldingRecognitionConfiguration cmd)
    {
        await proxy.SetRecognitionBorders(cmd.BrightPixelsBorder, cmd.DarkPixelsBorder);
        var state = new WeldingRecognitionConfigurationState()
        {
            WeldingValue = cmd.WeldingValue,
            NonWeldingValue = cmd.NonWeldingValue,
            DetectionEnabled = cmd.DetectionEnabled,
            BrightPixelsBorder = cmd.BrightPixelsBorder,
            DarkPixelsBorder = cmd.DarkPixelsBorder

        };
        await plumber.AppendState(state, hostName);
    }
    public async Task Handle(HostName hostName, DefineWeldingRecognitionConfiguration cmd)
    {
        await proxy.SetRecognitionBorders(cmd.BrightPixelsBorder, cmd.DarkPixelsBorder);
        var ev = new WeldingRecognitionConfiguration()
        {
            WeldingValue = cmd.WeldingValue,
            NonWeldingValue = cmd.NonWeldingValue,
            DetectionEnabled = cmd.DetectionEnabled,
            BrightPixelsBorder = cmd.BrightPixelsBorder,
            DarkPixelsBorder = cmd.DarkPixelsBorder

        };
        await plumber.AppendState(ev, hostName);
    }
}