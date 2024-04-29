using MicroPlumberd.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System;

namespace EventPi.Services.Camera.Cli
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddGrpc();
            builder.Services.AddSingleton<GrpcFrameFeaturesService>();
            builder.Services.AddSingleton<WeldingDetectorService>();
            builder.Services.AddSingleton<GrpcCppCameraProxy>();
            builder.Services.AddPlumberd();

            builder.WebHost.UseKestrel(
                options =>
                {
                    options.ListenAnyIP(
                        5000,
                        listenOptions => listenOptions.Protocols = HttpProtocols.Http2
                    );
                }
            );
            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();
            app.UseRouting();
      
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GrpcFrameFeaturesService>();
            });

            app.Services.GetRequiredService<GrpcFrameFeaturesService>();
            app.MapControllers();

            app.Run();
        }
    }
}
