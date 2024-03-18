using System.Net.NetworkInformation;
using System.Net;
using Makaretu.Dns;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace EventPi.Advertiser
{
    internal class RpiAdvertiseService : IHostedService
    {
        private CancellationTokenSource _cancellationTokenSource;
        private readonly ILogger<RpiAdvertiseService> _logger;
        private readonly ServiceProfile _serviceProfile;
        private readonly ServiceDiscovery _serviceDiscovery;
        private string _interfaceWifi;
        private readonly string _interfaceEthernet;

        public RpiAdvertiseService(ILogger<RpiAdvertiseService> logger, string serviceName, int port)
        {
            _logger = logger;

            _serviceProfile = new ServiceProfile(Dns.GetHostName(), "video.tcp", 9001);
            _serviceDiscovery = new ServiceDiscovery();
            _interfaceWifi = string.Empty;
            _interfaceEthernet = string.Empty;
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                switch (ni.Description)
                {
                    case "wlan0":
                        _interfaceWifi = GetInterfaceAddress(ni);
                        break;
                    case "eth0":
                        _interfaceEthernet = GetInterfaceAddress(ni);
                        break;
                }
            }

            _logger.LogInformation($"Interfaces:   Wifi:{_interfaceWifi};Ethernet:{_interfaceEthernet}");
            _serviceProfile.AddProperty("Wifi", $"{_interfaceWifi}");
            _serviceProfile.AddProperty("Ethernet", $"{_interfaceEthernet}");
        }

        private static string GetInterfaceAddress(NetworkInterface ni)
        {
            Console.WriteLine(ni.GetPhysicalAddress());
            foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork) continue;
                if (ip.Address.ToString() != string.Empty) return ip.Address.ToString();
            }

            return string.Empty;
        }

        private async Task AdvertiseThisRpi(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting advertising {Dns.GetHostName()} in local network!");


            while (!cancellationToken.IsCancellationRequested)
            {
                _serviceDiscovery.Advertise(_serviceProfile);
                _logger.LogInformation($"Advertised! Wifi:{_interfaceWifi} Ethernet:{_interfaceEthernet}");
                await Task.Delay(20000);
            }


        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_cancellationTokenSource != null)
                throw new InvalidOperationException("You cannot invoke start when already running (RpiAdvertiseService).");
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
