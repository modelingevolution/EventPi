using System.Diagnostics;
using EventPi.Pid;
using EventPi.Pwm.Ui.Components;
using Microsoft.AspNetCore.Mvc;
using EventPi.Pwm.Ui.Wasm.Client;
using EventPi.SignalProcessing;
using EventPi.SignalProcessing.Ui;
using _Imports = EventPi.Pwm.Ui.Wasm.Client._Imports;

namespace EventPi.Pwm.Ui
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var services = builder.Services;
            services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents(); 

            services.AddOpenApi()
                .AddSignalProcessingUi()
                .AddSignalsServer(s => s
                    .RegisterSink<float>("x-target")
                    .RegisterSink<float>("x-processed")
                    .RegisterSink<float>("x-error"))
                
                
                //.AddSingleton<NullPwmService>()
                .AddSingleton<DevicePwmService>()
                .AddSingleton<PidService>((sp) => new PidService(0.9,0,2,20,-20,2))
                .AddSingleton<IPwmService>(sp => sp.GetRequiredService<NullPwmService>())
                .AddHttpClient("default", sp =>
                    sp.BaseAddress = new Uri(builder.Configuration.GetValue<string>("Urls") ?? throw new InvalidOperationException("You need to configure Urls."))
                );

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
                app.UseExceptionHandler("/Error");
            }
            app.UseWebSockets();
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "api";
                options.SwaggerEndpoint("/openapi/v1.json", "v1");
            });
            app.MapOpenApi();
            app.UseAntiforgery();
            
            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(_Imports).Assembly)
                .AddAdditionalAssemblies(typeof(EventPi.SignalProcessing.Ui._Imports).Assembly); 
            ;
            app.MapSignals();
            
            app.MapPost("/Pwm/Parameters", PwmHandler.SetParameters);
            app.MapPost("/Pwm/Start", PwmHandler.Start);
            app.MapPost("/Pwm/Stop", PwmHandler.Stop);
            app.MapGet("/Pwm", PwmHandler.Get);
           
            app.MapPost("/Pid/Parameters", PidHandler.SetParameters);
            app.MapPost("/Pid/Compute", PidHandler.Compute);

            app.Run();
        }
    }

    
    public class PidHandler
    {
        public record Parameters(double Kp, double Ki, double Kd, double OutputUpperLimit, double OutputLowerLimit);

        public record Signal(double Input, double Expected);
        public static void SetParameters([FromServices] PidService srv, [FromBody] Parameters parameters)
        {
            srv.Kp = parameters.Kp;
            srv.Ki = parameters.Ki;
            srv.Kd = parameters.Kd;
            srv.OutputUpperLimit = parameters.OutputUpperLimit;
            srv.OutputLowerLimit = parameters.OutputLowerLimit;
        }

        
        public static void Compute([FromServices] PidService srv,[FromServices] IPwmService pwm,  [FromBody] Signal signal)
        {
            var result = srv.Compute(signal.Expected, signal.Input);
            pwm.DutyCycle = Math.Abs(result);
            pwm.IsReverse = result < 0;
        }
        public static Parameters Get([FromServices] PidService srv)
        {
            return new Parameters(srv.Kp, srv.Ki, srv.Kd, srv.OutputUpperLimit, srv.OutputLowerLimit);
        }
    }

    public class PidService : PidControllerTimeWrapper<PidController>
    {
        
        public PidService(double kp, double kd, double ki, double ou, double ol, double? ig) : base(new PidController(kp,kd,ki,ou,ol, ig))
        {
        }
        
    }
    public class NullPwmService : IPwmService
    {
        public bool IsRunning { get; private set; }
        public double DutyCycle { get; set; } = 0;
        public bool IsReverse { get; set; }
        public void Start()
        {
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public int Frequency { get; set; } = 16000;

    }
    public class PwmHandler
    {
        public record Parameters(double DutyCycle, int Frequency, bool IsReverse);
        
        public static void SetParameters([FromServices] IPwmService pwmService, Parameters p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));
            
            pwmService.DutyCycle = p.DutyCycle;
            pwmService.Frequency = p.Frequency;
            pwmService.IsReverse = p.IsReverse;

        }

        public static void Start([FromServices] IPwmService pwmService)
        {
            if (pwmService == null) throw new ArgumentNullException(nameof(pwmService));

            pwmService.Start();
        }

        public static void Stop([FromServices] IPwmService pwmService)
        {
            if (pwmService == null) throw new ArgumentNullException(nameof(pwmService));

            pwmService.Stop();
        }
        public static Parameters Get([FromServices] IPwmService pwmService)
        {
            if (pwmService == null) throw new ArgumentNullException(nameof(pwmService));

            return new Parameters(pwmService.DutyCycle, pwmService.Frequency, pwmService.IsReverse);
        }
    }
}
