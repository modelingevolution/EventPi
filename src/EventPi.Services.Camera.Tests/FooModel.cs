using MicroPlumberd;

namespace EventPi.Services.Camera.Tests;

[OutputStream("FooModel_v1")]
[EventHandler]
public partial class FooModel(InMemoryModelStore assertionModelStore)
{
    public InMemoryModelStore ModelStore => assertionModelStore;
    public async Task<string?> FindById(Guid id) => (await assertionModelStore.FindLast<CameraProfileConfigurationDefined>(id))?.Hostname;
    private async Task Given(Metadata m, CameraConfigurationProfileApplied ev)
    {
        assertionModelStore.Given(m, ev);
        await Task.Delay(0);
    }
    private async Task Given(Metadata m, CameraProfileConfigurationDefined ev)
    {
        assertionModelStore.Given(m, ev);
        await Task.Delay(0);
    }
}