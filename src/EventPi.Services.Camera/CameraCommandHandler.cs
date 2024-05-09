using EventStore.Client;
using MicroPlumberd;
using MicroPlumberd.Services;
using System.ComponentModel.DataAnnotations;
using EventPi.Abstractions;
using EventPi.Services.Camera.Contract;

namespace EventPi.Services.Camera;

[CommandHandler]
public partial class CameraCommandHandler(IPlumber plumber)
{
    public async Task Handle(HostProfilePath hostProfilePath, DefineProfileCameraHistogramFilter cmd)
    {
        var state = new CameraHistogramFilter()
        {
            Points = cmd.Points
        };
        await plumber.AppendState(state, hostProfilePath);
    }
    public async Task Handle(HostProfilePath hostProfilePath, DefineProfileCameraParameters cmd)
    {
        var ev = new CameraProfile()
        {
            Brightness = cmd.Brightness,
            Contrast = cmd.Contrast,
            Shutter = cmd.Shutter,
            Sharpness = cmd.Sharpness,
            AnalogueGain = cmd.AnalogueGain,
            DigitalGain = cmd.DigitalGain,
            CameraId = cmd.CameraId,
        };
        
        await plumber.AppendState(ev, hostProfilePath);
    }

    public async Task Handle(HostName hostName, SetCameraParameters cmd)
    {
        var ev = new CameraParametersState()
        {
            Brightness = cmd.Brightness,
            Contrast = cmd.Contrast,
            Shutter = cmd.Shutter,
            Sharpness = cmd.Sharpness,
            DigitalGain = cmd.DigitalGain,
            AnalogueGain = cmd.AnalogueGain,
            BlueGain = cmd.BlueGain,
            RedGain = cmd.RedGain
        };
        await plumber.AppendState(ev, hostName);
    }


}