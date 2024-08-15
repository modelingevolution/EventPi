using System.Diagnostics;
using FluentAssertions;
using Xunit.Abstractions;

namespace EventPi.NetworkMonitor.Tests
{
    public class NetworkManagerClientTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public NetworkManagerClientTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        //[Fact]
        //public async Task GetInterfaces()
        //{
        //    await using var client = await NetworkManagerClient.Create();
        //    var interfaces = client.GetDevices().ToBlockingEnumerable().ToArray();
        //    foreach (var i in interfaces)
        //    {
        //        _testOutputHelper.WriteLine(i.ToString());
        //    }

        //    interfaces.Should().HaveCountGreaterThan(1);
        //}
        //[Fact]
        //public async Task GetAvailableWifiNetworks()
        //{
        //    await using var client = await NetworkManagerClient.Create();
        //    var network = client.GetWifiNetworks().ToBlockingEnumerable();
        //    foreach (var i in network)
        //    {
        //        _testOutputHelper.WriteLine(i.ToString());
        //    }

        //    network.Should().HaveCountGreaterThan(0);
        //}

        //[Fact]
        //public async Task ConnectToNewWifi()
        //{
        //    await using var client = await NetworkManagerClient.Create();
        //    CancellationTokenSource cts = new CancellationTokenSource();

        //    var allNetwork = client.GetWifiNetworks().ToBlockingEnumerable().ToArray();
        //    var network = allNetwork.FirstOrDefault(x => x.Ssid == "Raptor");
        //    network.Should().NotBeNull();
        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();
        //    network.SourceDevice.StateChanged += (s, e) =>
        //    {
        //        _testOutputHelper.WriteLine($"{sw.Elapsed}: State changed: {e.OldState}->{e.NewState}");
        //        cts.Cancel();
        //    };
        //    await network.SourceDevice.SubscribeStateChanged();
        //    await network.Connect("foo");

        //    await Task.Delay(TimeSpan.FromSeconds(45));
        //    cts.IsCancellationRequested.Should().BeTrue();
        //}
        [Fact]
        public async Task ConnectToNewWifi2()
        {
            await using var client = await NetworkManagerClient.Create();
            CancellationTokenSource cts = new CancellationTokenSource();

            var allNetwork = client.GetWifiNetworks().ToBlockingEnumerable().ToArray();
            var network = allNetwork.FirstOrDefault(x => x.Ssid == "Raptor");
            network.Should().NotBeNull();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            network.SourceDevice.StateChanged += (s, e) =>
            {
                _testOutputHelper.WriteLine($"{sw.Elapsed}: State changed: {e.OldState}->{e.NewState}");
                if(e.NewState == DeviceStateChanged.Activated)
                    cts.Cancel();
            };
            await network.SourceDevice.SubscribeStateChanged();
            await network.Connect("");

            var action = async () => await Task.Delay(TimeSpan.FromSeconds(45), cts.Token);
            await action.Should().ThrowAsync<OperationCanceledException>();
        }
    }
}