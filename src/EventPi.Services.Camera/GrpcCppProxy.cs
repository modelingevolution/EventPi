using System.Threading.Channels;
using EventPi.Services.Camera.Contract;
using EventPi.Services.CameraAutoHistogram;
using EventPi.Services.CameraConfigurator;
using EventPi.Services.CameraFrameFeaturesConfigurator;
using EventPi.Services.CameraOptions;
using EventPi.Services.CameraShutter;
using EventPi.Services.CameraGreeter;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using static System.Net.WebRequestMethods;
using Emgu.CV.Ocl;

//TODO: refactor explicit metody

namespace EventPi.Services.Camera
{
    public interface ICameraManager : IDisposable
    {
        Task<bool> ProcessAsync(SetCameraHistogramFilter ev, int cameraNr = 0);
        Task<bool> ProcessAsync(ICameraParameters ev, int cameraNr = 0);
        Task<bool> GreetWithRpiCam(int camNr = 0);
        
    }

    public class ConsoleCameraManager(ILogger<ConsoleCameraManager> logger) : ICameraManager
    {
        public void Dispose()
        {
            
        }

        public async Task<bool> ProcessAsync(SetCameraHistogramFilter ev, int cameraNr = 0)
        {
            logger.LogInformation($"Process async [{cameraNr}]: {ev}");
            return true;
        }

        public async Task<bool> ProcessAsync(ICameraParameters ev, int cameraNr = 0)
        {
            logger.LogInformation($"Process async [{cameraNr}]: {ev}");
            return true;
        }

        public async Task<bool> GreetWithRpiCam(int camNr = 0)
        {
            logger.LogInformation($"Greet: {camNr}");
            return true;
        }
    }

    public class CameraManager : IDisposable, ICameraManager
    {
        private readonly Dictionary<int, GrpcChannel> _channels = new();
        private readonly ILogger<CameraManager> _logger;
        private readonly IConfiguration _config;
        private readonly Channel<Func<Task>> _queue = Channel.CreateUnbounded<Func<Task>>();
        private readonly CancellationTokenSource _cts = new();
        
        public CameraManager(ILogger<CameraManager> logger, IConfiguration config)
        {
            this._config = config;
            _logger = logger;
            Task.Factory.StartNew(OnRunQueue, TaskCreationOptions.LongRunning);
        }

        private async Task OnRunQueue()
        {
            try
            {
                await foreach (var i in _queue.Reader.ReadAllAsync(_cts.Token))
                    await i();
            }
            catch(OperationCanceledException) {}
        }
        private int CameraCount => _config.GetCameraCameraCount();

        private GrpcChannel GetClient(int cameraNr = 0)
        {
            if(_channels.TryGetValue(cameraNr, out GrpcChannel channel)) return channel;
            
            var ch = GrpcChannel.ForAddress($"http://{_config.GetLibcameraGrpcFullListenAddress(cameraNr)}");
            _channels.Add(cameraNr, ch);
            return ch;
        }

        public async Task<bool> ProcessAsync(SetCameraHistogramFilter ev, int cameraNr = 0)
        {
            return _queue.Writer.TryWrite(() => OnProcessAsyncWithRetry(ev, cameraNr));
        }
        private async Task OnProcessAsyncWithRetry(SetCameraHistogramFilter ev, int cameraNr = 0, int retry = 10, int delay = 1200)
        {
            for (int i = 0; i < retry; i++)
            {
                if (await OnProcessAsync(ev, cameraNr))
                    return;
                else
                    await Task.Delay(delay);
            }
            _logger.LogError($"Could not set camera histogram after {retry} retries: {cameraNr}");

        }

        private async Task<bool> OnProcessAsync(SetCameraHistogramFilter ev, int cameraNr = 0, int retry = 10,
            int delay = 1200)
        {
            var client = new CameraConfigurator.CameraConfigurator.CameraConfiguratorClient(GetClient(cameraNr));

            try
            {
                await client.ProcessAsync(new ConfigureHistogramRequest()
                {
                    ConfigurationName = ev.Id.ToString(),
                    Histogram = ByteString.CopyFrom(ev.Values)

                });
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("Couldn't set camera histogram via gRPC!");
            }

            return false;

        }

        public async Task<bool> GreetWithRpiCam(int camNr = 0)
        {
            try
            {
                var clientOptions = new CameraGreeter.CameraGreeter.CameraGreeterClient(GetClient(camNr));
                var result = clientOptions.ProcessAsync(new Empty()).GetAwaiter().GetResult();

                if (result.Config == "Hello!")
                    return true;
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError("Couldn't greet via gRPC!");
            }

            return false;

        }

        public async Task<bool> ProcessAsync(ICameraParameters ev, int cameraNr = 0)
        {
            return _queue.Writer.TryWrite(() => OnProcessAsyncWithRetry(ev, cameraNr));
        }

        private async Task OnProcessAsyncWithRetry(ICameraParameters ev, int cameraNr = 0, int retry = 10, int delay=1200)
        {
            for (int i = 0; i < retry; i++)
            {
                if (await OnProcessAsync(ev, cameraNr))
                    return;
                else
                    await Task.Delay(delay);
            }
            _logger.LogError($"Could not set camera parameters  after {retry} retries: {cameraNr}");
            
        }

        private async Task<bool> OnProcessAsync(ICameraParameters ev, int cameraNr = 0)
        {
            if (cameraNr == -1)
            {
                _logger.LogInformation($"Trying to set parameters to all {CameraCount} cameras.");
                bool success = true;
                for (int i = 0; i < CameraCount; i++)
                    try
                    {
                        success &= await ProcessAsync(ev, i);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Couldn't process camera at: {i}");
                    }

                return success;
            }

            bool error = false;
            try
            {
                var clientOptions = new CameraOptions.CameraOptions.CameraOptionsClient(GetClient(cameraNr));
                await clientOptions.ProcessAsync(new CameraOptionsRequest()
                {
                    AnologueGain = ev.AnalogueGain,
                    BlueGain = ev.BlueGain,
                    RedGain = ev.RedGain,
                    Brightness = ev.Brightness,
                    ColorMap = (int)ev.ColorMap,
                    Contrast = ev.Contrast,
                    DigitalGain = ev.DigitalGain,
                    Sharpness = ev.Sharpness,
                    HdrMode = (int)ev.HdrMode
                });
                
            }
            catch (Exception e)
            {
                error = true;
                _logger.LogError($"Couldn't send camera options via gRPC to camera {cameraNr}.");
            }

            try
            {
                var clientShutter = new CameraShutter.CameraShutter.CameraShutterClient(GetClient(cameraNr));
                await clientShutter.ProcessAsync(new ConfigureShutterRequest()
                {
                    Shutter = ev.Shutter,
                    ExposureLevel = ev.ExposureLevel

                });

            }
            catch (Exception e)
            {
                error = true;
                _logger.LogError($"Couldn't send camera shutter via gRPC to camera {cameraNr}.");
            }

            if (!error)
                _logger.LogInformation($"Set camera {cameraNr} completed with parameters: {ev}");
            else _logger.LogError($"Set camera (options or shutter) {cameraNr} failed with parameters: {ev}");
            

            return !error;
        }

        public void Dispose()
        {
            foreach(var v in _channels.Values)
                v.Dispose();
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
