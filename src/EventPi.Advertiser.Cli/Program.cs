using EventPi.Advertiser.Receiver;
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
                host.Configure(x => x.AddAdvertiser(new ServiceInfo("iot.www", 2113)));
                await host.Host.RunAsync();
            }
            else if (args.Contains("listen"))
            {
                Console.WriteLine("Listening...");
                AppHost host = new AppHost();
                host.Configure(x => x.AddLocalDiscoveryService("iot.www.local"));
                var sp = await host.StartAsync();
                sp.GetRequiredService<ILocalDiscoveryService>().ServiceFound += (s, e) => { Console.WriteLine($"Service found, host:{e.Hostname}, service: {e.ServiceName}, url:{e.Url}"); };
                Console.ReadLine();
            }
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
