using System.Collections.Concurrent;
using EventPi.Abstractions;
using MicroPlumberd;

namespace EventPi.Services.Camera;

[EventHandler]
public partial class CameraProfileConfigurationModel
{

    private CameraConfigurationProfile? _currentProfile;

    private ConcurrentDictionary<string, CameraConfigurationProfile> _availableProfiles;
    private readonly GrpcCppProxy _proxy;

    public CameraConfigurationProfile? CurrentProfile => _currentProfile;
    public CameraProfileConfigurationModel(IEnvironment env, GrpcCppProxy proxy)
    {
        _availableProfiles = new ConcurrentDictionary<string, CameraConfigurationProfile>();
        _proxy = proxy;
        _proxy.InitProxy();
    }
    private async Task Given(Metadata m, CameraProfileConfigurationDefined ev)
    {
        var profile = new CameraConfigurationProfile(ev);
       _availableProfiles.AddOrUpdate(profile.ProfileName,profile, (key, oldValue) => profile);
    }

    private async Task Given(Metadata m, CameraConfigurationProfileApplied ev)
    {
        var selectedProfile = _availableProfiles[ev.ProfileName];
        await _proxy.ProcessAsync(selectedProfile);
        _currentProfile = selectedProfile;
    }
}