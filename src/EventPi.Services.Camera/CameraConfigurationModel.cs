using System.Collections;
using System.Collections.Concurrent;
using EventPi.Abstractions;
using EventPi.Services.Camera.Contract;
using MicroPlumberd;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EventPi.Services.Camera;



[EventHandler]
public partial class CameraProfileConfigurationModel
{
    private ICameraParametersReadOnly _weldingProfile;
    private ICameraParametersReadOnly _defaultProfile;

    public ICameraParametersReadOnly WeldingProfile => _weldingProfile;

    public ICameraParametersReadOnly DefaultProfile => _defaultProfile;
 
    public CameraProfileConfigurationModel()
    {
        _weldingProfile = new SetCameraParameters();
        _defaultProfile = new SetCameraParameters();
    }

    private async Task Given(Metadata m, CameraProfile ev)
    {
        var id = m.StreamId<HostProfilePath>();
        var profile = new CameraConfigurationProfile(ev);

        if (id.ProfileName.Equals("welding", StringComparison.InvariantCultureIgnoreCase))
        {
            _weldingProfile = profile;
        }
        else
        {
            _defaultProfile = profile;
        }
        
    }

}