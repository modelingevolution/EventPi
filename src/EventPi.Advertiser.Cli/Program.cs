using EventPi.Advertiser.Receiver;
using EventPi.Advertiser.Sender;
using Makaretu.Dns;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventPi.Advertiser.Cli
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Contains("advertise"))
            {
                Console.WriteLine("Advertising...");
                AppHost host = new AppHost();
                host.Configure(x => 
                    //x.AddSingleton<IServiceProfileEnricher,LocalhostEnricher>()
                        x.AddAdvertiser(new ServiceInfo("http","iot.www", 8080)));
                await host.Host.RunAsync();
            }
            else if (args.Contains("listen"))
            {
                Console.WriteLine("Listening...");
                AppHost host = new AppHost();
                host.Configure(x => x.AddLocalDiscoveryService("iot.www.local"));
                var sp = await host.StartAsync();
                sp.GetRequiredService<ILocalDiscoveryService>().ServiceFound += (s, e) =>
                {
                    Console.WriteLine("Received adv.");
                    var c = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Service found, host:{e.Hostname}, service: {e.ServiceName}, url: {e.Url}");
                    Console.ForegroundColor = c;
                };

                Console.ReadLine();
            }
        }
    }

    class LocalhostEnricher : IServiceProfileEnricher
    {
        public void Enrich(ServiceProfile profile)
        {
            profile.AddProperty("Ethernet", "192.168.30.27");
        }
    }
    public partial class AppHost : IDisposable
    {
        public IHost Host { get; private set; }
        

        public AppHost()
        {
            
        }

        public virtual AppHost Configure(Action<IServiceCollection>? configure = null)
        {
            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureLogging(x => x.SetMinimumLevel(LogLevel.Trace).AddDebug())
                .ConfigureServices(services =>
                {
                    configure(services);
                })
                .Build();

            return this;
        }

        public void Dispose()
        {
            Host?.Dispose();
        }


        public async Task<IServiceProvider> StartAsync()
        {
            await Host.StartAsync();
            await Task.Delay(1000);
            return Host.Services;
        }
    }
}
