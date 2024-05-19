using System.Net;
using Makaretu.Dns;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Zeroconf;

namespace EventPi.Advertiser.Sender
{

    internal class AdvertiserService : BackgroundService
    {
        private CancellationTokenSource _cancellationTokenSource;
        private readonly IServer _host;
        private readonly ILogger<AdvertiserService> _logger;
        private readonly IServiceProfileEnricher _enricher;
        private readonly ServiceInfo[] _services;
        private readonly List<AdvertiseSender> _advertisers;


        public AdvertiserService(IServer host, ILogger<AdvertiserService> logger, 
            IServiceProfileEnricher enricher, IEnumerable<ServiceInfo> services)
        {
            _host = host;
            _logger = logger;
            _enricher = enricher;
            _services = services.ToArray();
            _advertisers = new List<AdvertiseSender>();
            

        }

        private async Task Do(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting advertising {Dns.GetHostName()} in local network!");
            while (!cancellationToken.IsCancellationRequested)
            {
                foreach (var service in _advertisers)
                {
                    service.Advertise();
                    await Task.Delay(500, cancellationToken);
                }
                await Task.Delay(20000, cancellationToken);
            }


        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();
            await Task.Delay(2000,stoppingToken); // wait for the server to start
            Init();
            await Do(stoppingToken);
        }

        private void Init()
        {
            var urls = _host.Features.Get<IServerAddressesFeature>();
            var ports = urls.Addresses.Select(x => new Uri(x)).Select(x => x.Port).Distinct().ToArray();
            foreach (var service in _services)
            {
                if (!service.Port.HasValue)
                    foreach (var i in ports)
                        CreateAdvertiser(service, i);
                else
                    CreateAdvertiser(service, service.Port.Value);
            }
        }

        private void CreateAdvertiser(ServiceInfo service, int port)
        {
            _logger.LogInformation(
                $"Advertising {service.ServiceName}: {service.Schema}://{{IP}}:{port}");
            _advertisers.Add(AdvertiseSender.Create(_enricher, Dns.GetHostName(),
                service.Schema, service.ServiceName, port, service.Properties));
        }
    }
}
