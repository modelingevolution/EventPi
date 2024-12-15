using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ProtoBuf.Meta;

namespace EventPi.Pwm.Ui.Wasm.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            var baseAddress = builder.HostEnvironment.BaseAddress;
            
            builder.Services.AddHttpClient("default",
                (provider, client) => client.BaseAddress = new Uri(baseAddress));
            
            builder.Services.AddSignalWasm();
            
            await builder.Build().RunAsync();
        }


    }
}
