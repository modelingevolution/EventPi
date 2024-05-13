using EventPi.NetworkMonitor.App.Blazor.Components;
using EventPi.Services.NetworkMonitor;
using MicroPlumberd.Services;
using MudBlazor.Services;
using System.Diagnostics;
using EventPi.Abstractions;
using EventPi.Services.NetworkMonitor.Ui;
using EventStore.Client;

namespace EventPi.NetworkMonitor.App.Blazor
{
    public class Program
    {
        private static async Task CheckDebugger(string[] args)
        {
            if (Environment.GetEnvironmentVariable("DEBUG") != null || args.Contains("--debug"))
            {
                if (args.Contains("--disableDebug")) return;

                Console.WriteLine("Waiting for debugger to attach.");
                while (!Debugger.IsAttached)
                {
                    await Task.Delay(500);
                }
                Console.WriteLine("Application is attached to debugger.");
            }
        }
        public static async Task Main(string[] args)
        {
            await CheckDebugger(args);
            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.
            var services = builder.Services;

            bool disableUi = args.Contains("--disableUi");
            if(disableUi) Console.WriteLine("UI is disabled.");

            services.AddEventPiAbstractions()
                .AddPlumberd(sp => EventStoreClientSettings.Create(sp.GetRequiredService<IConfiguration>().GetValue<string>("EventStore")!));

            if (!disableUi)
            {
                services.AddMudServices()
                    .AddNetworkManagerUi()
                    .AddPlumberd(sp => EventStoreClientSettings.Create(sp.GetRequiredService<IConfiguration>().GetValue<string>("EventStore")!))
                    .AddRazorComponents()
                    .AddInteractiveServerComponents();
            }

            if (Environment.OSVersion.Platform == PlatformID.Unix)
                services.AddNetworkManager();

            var app = builder.Build();

            if (!disableUi)
            {
                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Error");
                }

                app.UseStaticFiles();
                app.UseAntiforgery();

                app.MapRazorComponents<Components.App>()
                    .AddInteractiveServerRenderMode();
            }

            await app.RunAsync();
        }
    }
}
