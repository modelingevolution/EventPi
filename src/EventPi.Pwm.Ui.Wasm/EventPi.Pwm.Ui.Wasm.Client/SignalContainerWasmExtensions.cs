using EventPi.SignalProcessing;

namespace EventPi.Pwm.Ui.Wasm.Client;

public static class SignalContainerWasmExtensions
{
    public static IServiceCollection AddSignalWasm(this IServiceCollection services)
    {
        services.AddScoped<SignalHubClient>();
        return services;
    }
}