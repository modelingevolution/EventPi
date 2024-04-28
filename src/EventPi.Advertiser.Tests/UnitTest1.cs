using EventPi.Advertiser.Receiver;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using Zeroconf;
using FluentAssertions;

namespace EventPi.Advertiser.Tests
{
    public class AdvertiserTests(ITestOutputHelper logger)
    {
        private TestAppHost _hostClient = new TestAppHost(logger);
        private TestAppHost _hostServer = new TestAppHost(logger);


        [Fact]
        public async Task AdvertiseAndReceiveServices()
        {
            
            _hostClient.Configure(x => x.AddAdvertiser( new ServiceInfo("Video.tcp", 9001), new ServiceInfo("EventStore.tcp", 2113)));
            await _hostClient.StartAsync();

            _hostServer.Configure(x => x.AddLocalDiscoveryService("Video.tcp","EventStore.tcp"));
            await _hostServer.StartAsync();

            CancellationTokenSource cst = new CancellationTokenSource();
            _hostServer.Host.Services.GetRequiredService<ILocalDiscoveryService>().ServiceFound +=
                (s, e) => { cst.Cancel(); };


            await Task.Delay(TimeSpan.FromSeconds(30), cst.Token);
            cst.IsCancellationRequested.Should().BeTrue();

        }

      
    }

}