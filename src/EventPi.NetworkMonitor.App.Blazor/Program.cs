using EventPi.NetworkMonitor.App.Blazor.Components;
using EventPi.Services.NetworkMonitor;
using MicroPlumberd.Services;
using MudBlazor.Services;
using System.Diagnostics;

namespace EventPi.NetworkMonitor.App.Blazor
{
    public class Program
    {
        private static async Task CheckDebugger()
        {
            if (Environment.GetEnvironmentVariable("DEBUG") != null)
            {
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
            await CheckDebugger();
            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.
            var services = builder.Services;
            
            services.AddMudServices()
                .AddNetworkManager()
                .AddPlumberd()
                .AddRazorComponents()
                .AddInteractiveServerComponents();

            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<Components.App>()
                .AddInteractiveServerRenderMode();

            await app.RunAsync();
        }
    }
}
