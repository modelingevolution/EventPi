using EventStore.Client;
using MicroPlumberd;
using MicroPlumberd.Services;
using System.ComponentModel.DataAnnotations;

namespace EventPi.Services.Camera;


[CommandHandler]
public partial class CameraProfileConfigurationCommandHandler(IPlumber plumber)
{
    public async Task Handle(Guid id, DefineProfileConfiguration cmd)
    {
        var ev = new CameraProfileConfigurationDefined()
        {
            Hostname = cmd.Hostname,
            Brightness = cmd.Brightness,
            Contrast = cmd.Contrast,
            Shutter = cmd.Shutter,
            Sharpness = cmd.Sharpness,
            AnalogGain = cmd.AnalogGain,
            DigitalGain = cmd.DigitalGain,
            CameraId = cmd.CameraId,
            Profile = cmd.Profile,
        };
        
        await plumber.AppendEvent(ev);
    }
  


}