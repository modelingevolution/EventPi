using EventPi.Services.Camera.Contract;
using MicroPlumberd;

namespace EventPi.Services.Camera;

[EventHandler]
public partial class CameraProfileModel
{
    public CameraProfile Profile { get; private set; }
    private async Task Given(Metadata m, CameraProfile ev)
    {
        Profile = ev;
    }
}
