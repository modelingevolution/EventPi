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
                string pwd = args[2];

                await using var client = await NetworkManagerClient.Create();
                CancellationTokenSource cts = new CancellationTokenSource();


                AccessPointInfo? network = null;
                await client.RequestWifiScan();
                await foreach (var i in client.GetAccessPoints())
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
                    if (e.NewState == DeviceState.Activated)
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
                await foreach (var i in client.GetProfiles())
                {
                    if(!i.FileName.Contains("prec", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    Console.WriteLine(i);
                    var wifiSettings = await i.Settings();
                    if(wifiSettings!= null)
                        Console.WriteLine(i);
                }
                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();
            }
            else if (args.Length == 3 && args[0] == "add")
            {
                string ssid = args[1];
                string pwd = args[2];

                await using var client = await NetworkManagerClient.Create();
                CancellationTokenSource cts = new CancellationTokenSource();


                AccessPointInfo? network = null;
                await client.RequestWifiScan();
                await foreach (var i in client.GetAccessPoints())
                {
                    Console.WriteLine($"Network found: {i}");
                    if (i.Ssid == ssid) network = i;
                }

                if (network == null)
                {
                    Console.Error.WriteLine($"No network '{ssid}' found");
                    return;
                }
                else Console.WriteLine($"Wifi network '{ssid}' found.");

                //var r = await network.Setup(pwd, ssid + "-connection");
                //Console.WriteLine($"Connection: {r.FileName} created.");
            } 
            else if(args.Length == 3 && args[0] == "connection" && args[1] == "up")
            {
                var connection = args[2];
                await using var client = await NetworkManagerClient.Create();
                await client.ActivateConnection(connection);
                Console.WriteLine($"Connection is up: {connection}");
            }
            else if (args.Length == 3 && args[0] == "connection" && args[1] == "down")
            {
                var connection = args[2];

                await using var client = await NetworkManagerClient.Create();
                await client.DisableConnection(connection);

                Console.WriteLine($"Connection is down: {connection}");
            }
            else if (args.Length == 3 && args[0] == "connection" && args[1] == "check")
            {
                var connection = args[2];

                await using var client = await NetworkManagerClient.Create();
                var st = (await client.IsConnectionActive(connection)) ? "up" : "down";

                Console.WriteLine($"Connection: {connection} is {st}");
            }
        }
    }
}
