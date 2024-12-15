using System.Device.Pwm;
using System.Diagnostics;
using EventPi.Pid;
using EventPi.Pwm.Ui.Components;
using Microsoft.AspNetCore.Mvc;
using EventPi.Pwm.Ui.Wasm.Client;
using EventPi.SignalProcessing;
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
                .AddSignalWasm()
                .AddSignalsServer(s => s
                    .RegisterSink<float>("input")
                    .RegisterSink<float>("output"))
                
                
                .AddSingleton<NullPwmService>()
                .AddSingleton<PidService>()
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
                .AddAdditionalAssemblies(typeof(_Imports).Assembly); 
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
            var result = srv.Compute(signal.Input, signal.Expected);
            pwm.DutyCycle = Math.Abs(result);
            pwm.IsReverse = result < 0;
        }
        public static Parameters Get([FromServices] PidService srv)
        {
            return new Parameters(srv.Kp, srv.Ki, srv.Kd, srv.OutputUpperLimit, srv.OutputLowerLimit);
        }
    }

    public class PidService
    {
        private readonly PidController _controller = new(0.2, 0.01, 0.001, 0.5, 0);
        private Stopwatch? _dt;

        public double Compute(double input, double expected)
        {
            if (_dt == null)
            {
                _dt = Stopwatch.StartNew();
                return 0;
            }
            var value = _controller.CalculateOutput(expected, input, _dt.Elapsed);
            _dt.Restart();
            return value;
        }
        public double Kp
        {
            get => _controller.Kp;
            set => _controller.Kp = value;
        }

        public double Ki
        {
            get => _controller.Ki;
            set => _controller.Ki = value;
        }

        public double Kd
        {
            get => _controller.Kd;
            set => _controller.Kd = value;
        }

        public double OutputUpperLimit
        {
            get => _controller.OutputUpperLimit;
            set => _controller.OutputUpperLimit = value;
        }

        public double OutputLowerLimit
        {
            get => _controller.OutputLowerLimit;
            set => _controller.OutputLowerLimit = value;
        }
    }

    public interface IPwmService
    {
        bool IsReverse { get; set; }
        bool IsRunning { get; }
        void Start();
        void Stop();
        int Frequency { get; set; }
        double DutyCycle { get; set; }
    }

    public class DevicePwmService : IPwmService
    {
        readonly PwmChannel _channel;
        public bool IsReverse { get; set; }
        public bool IsRunning { get; private set; }
        public void Start()
        {
            _channel.Start();
            IsRunning = true;
        }

        public void Stop()
        {
            _channel.Stop();
            IsRunning = false;
        }

        public int Frequency
        {
            get => _channel.Frequency;
            set => _channel.Frequency = value;
        }

        public double DutyCycle
        {
            get => _channel.DutyCycle;
            set => _channel.DutyCycle = value;
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
