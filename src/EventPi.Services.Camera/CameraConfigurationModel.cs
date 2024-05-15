using System.Collections.Concurrent;
using EventPi.Abstractions;
using EventPi.Services.Camera.Contract;
using MicroPlumberd;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EventPi.Services.Camera;

[EventHandler]
public partial class CameraProfileConfigurationModel
{

    private CameraConfigurationProfile? _currentProfile;

    private ConcurrentDictionary<string, CameraConfigurationProfile> _availableProfiles;
    private readonly GrpcCppCameraProxy _cameraProxy;

    public CameraConfigurationProfile? CurrentProfile => _currentProfile;
    public CameraProfileConfigurationModel(IEnvironment env, GrpcCppCameraProxy cameraProxy)
    {
        _availableProfiles = new ConcurrentDictionary<string, CameraConfigurationProfile>();
        _cameraProxy = cameraProxy;
    }

    public CameraConfigurationProfile? GetProfileByName(string name)
    {
        _availableProfiles.TryGetValue(name, out var profile);
        return profile;
    }
    private async Task Given(Metadata m, CameraProfile ev)
    {
        var id = m.StreamId<HostProfilePath>();
        var profile = new CameraConfigurationProfile(ev);
       _availableProfiles.AddOrUpdate(id.ProfileName, profile, (key, oldValue) => profile);
    }

    private async Task Given(Metadata m, CameraConfigurationProfileApplied ev)
    {
        var selectedProfile = _availableProfiles[ev.ProfileName];
       // await _cameraProxy.ProcessAsync(selectedProfile);
        _currentProfile = selectedProfile;
    }
}