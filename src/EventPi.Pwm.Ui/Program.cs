using System.Diagnostics;
using System.Text;
using System.Text.Json;
using EventPi.Pid;
using EventPi.Pwm.Ui.Components;
using Microsoft.AspNetCore.Mvc;
using EventPi.Pwm.Ui.Wasm.Client;
using EventPi.SignalProcessing;
using EventPi.SignalProcessing.Ui;
using _Imports = EventPi.Pwm.Ui.Wasm.Client._Imports;
using Microsoft.AspNetCore.SignalR;
using System.Xml.Linq;

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
                    .RegisterSink<float>("x-short-target")
                    .RegisterSink<float>("x-processed")
                    .RegisterSink<float>("x-error")
                    .RegisterSink<float>("x-motor")
                    .RegisterSink<float>("x-prediction")
                    
                    )
                
                
                .AddSingleton<NullPwmService>()
                //.AddSingleton<DevicePwmService>()
                .AddSingleton<StepMotorController>()
                .AddSingleton<StepMotorSignalsObserver>()
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

            app.MapPost("/Motor/{name}", MotorHandler.Steer);
            app.MapPut("/Motor/{name}", MotorHandler.RegisterMotor);
            app.MapPost("/Motor/{name}/Observe", MotorHandler.ObserveMotor);

            app.Run();
        }
    }

    public class StepMotorSignalsObserver : IDisposable
    {
        private readonly SignalHubServer hub;
        private readonly StepMotorController _srv;
        private readonly IPwmService _pwm;
        private ISignalSink<float> targetSink;
        private ISignalSink<float> actualSink;
        private ISignalSink<float> errorSink;
        private ISignalSink<float> predictionSink;
        private ISignalSink<float> motorSink;
        private ISignalSink<float> shortTargetSink;
        private PeriodicTimer _pt;
        private CancellationTokenSource _cts;
        

        public StepMotorSignalsObserver(SignalHubServer hub, StepMotorController srv, IPwmService pwm)
        {
            this.hub = hub;
            _srv = srv;
            _pwm = pwm;
        }
        public bool IsRunning { get; private set; }
        public void Start(string name)
        {
            if (IsRunning) return;
            IsRunning = true;
            
            this.targetSink = hub.GetSink<float>(name + "-target");
            this.shortTargetSink = hub.GetSink<float>(name + "-short-target");
            this.actualSink = hub.GetSink<float>(name + "-processed");
            this.errorSink = hub.GetSink<float>(name + "-error");
            this.predictionSink = hub.GetSink<float>(name + "-prediction");
            this.motorSink = hub.GetSink<float>(name + "-motor");
            _pt = new PeriodicTimer(TimeSpan.FromSeconds(1d / 60));
            _cts = new CancellationTokenSource();
            _ = Task.Factory.StartNew(Run, TaskCreationOptions.LongRunning);
        }
        private async Task Run()
        {
            var motor = _srv.MotorModel;
            try
            {
                while (!_cts.IsCancellationRequested)
                {
                    await _pt.WaitForNextTickAsync(_cts.Token);
                    var target = _srv.Target;
                    var shortTarget = _srv.MotorModel.Target;
                    var processed = _srv.ProcessedValue;
                    var error = target - processed;
                    var prediction = motor.Position();
                    float pwmValue = 0f;
                    if (_pwm.IsRunning)
                        pwmValue = _pwm.IsReverse ? -1f : 1f;

                    shortTargetSink.Write((float)shortTarget);
                    motorSink.Write(pwmValue);
                    actualSink.Write((float)processed);
                    targetSink.Write((float)target);
                    errorSink.Write((float)error);
                    predictionSink.Write((float)prediction);
                }
            }
            catch (OperationCanceledException ex)
            {
                
            }

        }

        private bool _isDisposed;
        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            if(IsRunning)
                _cts.Cancel();
            _srv?.Dispose();
            _pt?.Dispose();
            _cts?.Dispose();
        }
    }

    public class MotorClient(string url)
    {
        readonly record struct Args(float Target, float Actual);
        // HttpProxy
        private readonly HttpClient _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(url)
        };


        public async Task Steer(string motorName, float target, float actual)
        {
            var response = await _httpClient.PostAsJsonAsync($"Motor/{motorName}", new Args(target,actual));
            response.EnsureSuccessStatusCode();
        }
    }
    public class MotorHandler
    {
        public readonly record struct Args(float Target, float Actual);

        public static void ObserveMotor([FromServices] StepMotorSignalsObserver observer, string? name)
        {
            name ??= "x";
            observer.Start(name);
        }
        public static void RegisterMotor([FromServices] SignalHubServer hub, 
            string name)
        {
            hub.RegisterSink<float>(name + "-target");
            hub.RegisterSink<float>(name + "-short-target");
            hub.RegisterSink<float>(name + "-processed");
            hub.RegisterSink<float>(name + "-error");
            hub.RegisterSink<float>(name + "-prediction");
            hub.RegisterSink<float>(name + "-motor");
        }
        public static SteerResponse Steer([FromServices] StepMotorController srv,
            [FromServices] SignalHubServer hub, string? name, 
            [FromServices] IPwmService pwm,
            [FromServices] ILogger<MotorHandler> logger,
            [FromBody]  Args arg)
        {
            name ??= "x";
            var targetSink =     hub.GetSink<float>(name + "-target");
            var shortTargetSink =hub.GetSink<float>(name + "-short-target");
            var actualSink =     hub.GetSink<float>(name + "-processed");
            var errorSink =      hub.GetSink<float>(name + "-error");
            var predictionSink = hub.GetSink<float>(name + "-prediction");
            var motorSink =      hub.GetSink<float>(name + "-motor");

            shortTargetSink.Write((float)srv.MotorModel.Target);
            var prediction = srv.MotorModel.Position();
            srv.MoveTo(arg.Target, arg.Actual);

            float pwmValue = 0f;
            if (pwm.IsRunning)
                pwmValue = pwm.IsReverse ? -1f : 1f;
            
            predictionSink.Write((float)prediction);
            targetSink.Write(arg.Target);
            actualSink.Write(arg.Actual);
            motorSink.Write(pwmValue);
            errorSink.Write(arg.Target - arg.Actual);
            
            // write all 5 values to logger
            logger.LogInformation(
                "Target: {Target}, Actual: {Actual}, Prediction: {Prediction}, Motor: {Motor}, Error: {Error}",
                arg.Target, arg.Actual, prediction, pwmValue, arg.Target - arg.Actual);
            // return anonymous type with all those 5 values and named properties
            return new SteerResponse()
            {
                Target = arg.Target,
                Actual = arg.Actual,
                Prediction = (float)prediction,
                Motor = pwmValue,
                Error = arg.Target - arg.Actual
            };
            
            
        }

        public readonly record struct SteerResponse(
            float Target,
            float Actual,
            float Prediction,
            float Motor,
            float Error);
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
    public class NullPwmService(ILogger<NullPwmService> logger) : IPwmService
    {
        private Stopwatch? _sw;
        private bool _isReverse;
        public bool IsRunning { get; private set; }
        public double DutyCycle { get; set; } = 0;
        public TimeSpan Worked => _sw?.Elapsed ?? TimeSpan.Zero;
       
        public bool IsReverse
        {
            get => _isReverse;
            set
            {
                if (_isReverse == value) return;
                _isReverse = value;
                logger.LogInformation("Pwm is-reverse changed: " + value.ToString());
            }
        }

        public void Start()
        {
            if (IsRunning) return;
            IsRunning = true;
            _sw = Stopwatch.StartNew();
            logger.LogInformation("Pwm started.");
        }

        public void Stop()
        {
            if (!IsRunning) return;
                
            IsRunning = false;
            _sw.Stop();
            logger.LogInformation($"Pwm stopped. Worked for {_sw.ElapsedMilliseconds} ms.");
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
