using System.Diagnostics;

namespace EventPi.NetworkMonitor.Cli
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (Environment.GetEnvironmentVariable("DEBUG") != null)
            {
                Console.WriteLine("Waiting for debugger to attach.");
                while (!Debugger.IsAttached)
                {
                    await Task.Delay(500);
                }
            }
            if (args.Length == 3 && args[0] == "connect")
            {
                string ssid = args[1];
                string pwd = args[1];

                await using var client = await NetworkManagerClient.Create();
                CancellationTokenSource cts = new CancellationTokenSource();


                WifiNetwork? network = null;
                await client.RequestWifiScan();
                await foreach (var i in client.GetWifiNetworks())
                {
                    Console.WriteLine($"Network found: {i}");
                    if (i.Ssid == ssid) network = i;
                }

                if (network == null)
                {
                    Console.Error.WriteLine($"No network '{ssid}' found");
                    return;
                } else Console.WriteLine($"Wifi network '{ssid}' found.");

                Stopwatch sw = new Stopwatch();
                sw.Start();
                network.SourceDevice.StateChanged += (s, e) =>
                {
                    Console.WriteLine($"{sw.Elapsed}: State changed: {e.OldState}->{e.NewState}");
                    if (e.NewState == DeviceStateChanged.Activated)
                        cts.Cancel();
                };
                await using var sub = await network.SourceDevice.SubscribeStateChanged();
                Console.WriteLine("Subscribe to state changes.");
                await network.Connect(pwd);
                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();
            } 
            else if (args.Length == 2 && args[0] == "list" && args[1] == "connections")
            {
                await using var client = await NetworkManagerClient.Create();
                await foreach (var i in client.GetConnections())
                {
                    Console.WriteLine(i);
                    var wifiSettings = await i.WifiSettings();
                    if(wifiSettings!= null)
                        Console.WriteLine(i);
                }
                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();
            }
        }
    }
}
