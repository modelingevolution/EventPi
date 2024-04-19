using System.Net;
using Makaretu.Dns;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace EventPi.Advertiser
{

    internal class AdvertiserService : IHostedService
    {
        private CancellationTokenSource _cancellationTokenSource;
        private readonly ILogger<AdvertiserService> _logger;
        private readonly RpiAdvertiseSender _advertiser;


        public AdvertiserService(ILogger<AdvertiserService> logger, IEnumerable<IServiceInfo> services)
        {
            _logger = logger;
           _advertiser = RpiAdvertiseSender.Create(Dns.GetHostName(),"video.tcp", 9001);

        }

        private async Task AdvertiseThisRpi(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting advertising {Dns.GetHostName()} in local network!");
            while (!cancellationToken.IsCancellationRequested)
            {
                _advertiser.Advertise();
                await Task.Delay(20000);
            }


        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_cancellationTokenSource != null)
                throw new InvalidOperationException("You cannot invoke start when already running (AdvertiserService).");
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            Task.Factory.StartNew(async () => await AdvertiseThisRpi(_cancellationTokenSource.Token), TaskCreationOptions.LongRunning);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource = null;
        }
    }
}
