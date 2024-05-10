using EventPi.Abstractions;
using EventPi.NetworkMonitor;
using EventPi.Services.NetworkMonitor.Contract;
using MicroPlumberd;
using MicroPlumberd.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventPi.Services.NetworkMonitor
{
    [CommandHandler]
    public partial class NetworkManagerCommandHandler
    {
        public async Task Handle(string profile, DefineWirelessProfile data)
        {

        }
    }

    public static class ContainerExtensions
    {
        public static IServiceCollection AddNetworkManager(this IServiceCollection services)
        {
            services.AddBackgroundServiceIfMissing<NetworkManagerListener>();
            return services;
        }
    }
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class NetworkManagerListener(IPlumber plumber, IEnvironment env) : BackgroundService
    {
        private readonly NetworkManagerClient _client = new();
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            WirelessStationsState state = new WirelessStationsState();
            await foreach (var i in _client.GetWifiNetworks().WithCancellation(stoppingToken))
            {
                state.Add(new ()
                {
                    InterfaceName = i.SourceInterface, Signal = i.SignalStrength, Ssid = i.Ssid
                });
            }

            await plumber.AppendState(state, env.HostName, token: stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.DisposeAsync();
        }

        
    }
}
