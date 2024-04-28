using System.Net;
using Makaretu.Dns;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace EventPi.Advertiser.Sender
{

    internal class AdvertiserService : IHostedService
    {
        private CancellationTokenSource _cancellationTokenSource;
        private readonly ILogger<AdvertiserService> _logger;
        private readonly List<AdvertiseSender> _advertisers;


        public AdvertiserService(ILogger<AdvertiserService> logger, IServiceProfileEnricher enrichers, IEnumerable<IServiceInfo> services)
        {
            _logger = logger;
            _advertisers = new List<AdvertiseSender>();
            foreach (var service in services)
            {
                _advertisers.Add(AdvertiseSender.Create(enrichers, service.Schema, Dns.GetHostName(),  service.ServiceName, service.Port));
                logger.LogInformation($"Advertising {service.ServiceName}: {service.Schema}://{{IP}}:{service.Port}");
            }

        }

        private async Task AdvertiseRegisteredServices(CancellationToken cancellationToken)
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
        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_cancellationTokenSource != null)
                throw new InvalidOperationException("You cannot invoke start when already running (AdvertiserService).");
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            Task.Factory.StartNew(async () => await AdvertiseRegisteredServices(_cancellationTokenSource.Token), TaskCreationOptions.LongRunning);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource = null;
        }
    }
}
