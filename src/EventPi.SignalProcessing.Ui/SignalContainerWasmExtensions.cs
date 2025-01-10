using Microsoft.Extensions.DependencyInjection;

namespace EventPi.SignalProcessing.Ui;

public static class SignalContainerWasmExtensions
{
    public static IServiceCollection AddSignalProcessingUi(this IServiceCollection services)
    {
        services.AddScoped<SignalHubClient>();
        return services;
    }
}