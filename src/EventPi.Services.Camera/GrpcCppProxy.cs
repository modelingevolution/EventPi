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
    public class GrpcCppCameraProxy : IDisposable
    {

        private readonly Dictionary<int, GrpcChannel> _channels = new();
        private readonly ILogger<GrpcCppCameraProxy> _logger;
        private readonly IConfiguration _config;
        public GrpcCppCameraProxy(ILogger<GrpcCppCameraProxy> logger, IConfiguration config)
        {
            this._config = config;
            _logger = logger;
        }
        private int CameraCount { get => _config.GetLibCameraCameraCount(); }
        private GrpcChannel GetClient(int cameraNr = 0)
        {
            if(_channels.TryGetValue(cameraNr, out GrpcChannel channel)) return channel;
            
            var ch = GrpcChannel.ForAddress($"http://{_config.GetLibcameraGrpcFullListenAddress(cameraNr)}");
            _channels.Add(cameraNr, ch);
            return ch;
        }


        public async Task<Empty> ProcessAsync(SetCameraHistogramFilter ev, int cameraNr = 0)
        {
            var client = new CameraConfigurator.CameraConfigurator.CameraConfiguratorClient(GetClient(cameraNr));

            try
            {
                await client.ProcessAsync(new ConfigureHistogramRequest()
                {
                    ConfigurationName = ev.Id.ToString(),
                    Histogram =  ByteString.CopyFrom(ev.Values)

                });
            }
            catch (Exception e)
            {
                _logger.LogError("Couldn't set camera histogram via gRPC!");
            }
           
        return new Empty();

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
   
        public async Task<Empty> ProcessAsync(ICameraParameters ev, int cameraNr = 0)
        {
            if(cameraNr == -1)
            {
                for (int i = 0; i < CameraCount; i++)
                    try
                    {
                        await ProcessAsync(ev, i);
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex,$"Couldn't process camera at: {i}");
                    }
                return new Empty();
            }

            _logger.LogInformation($"Trying to set parameters to camera {cameraNr}.");

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
                _logger.LogError("Couldn't send camera options via gRPC!");
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
                _logger.LogError("Couldn't send camera shutter via gRPC!");
            }

            try
            {
                var clientAutoHistogram =
                    new CameraAutoHistogram.CameraAutoHistogram.CameraAutoHistogramClient(GetClient(cameraNr));
                await clientAutoHistogram.ProcessAsync(new CameraAutoHistogramRequest()
                {
                    EnableAutoHistogram = ev.AutoHistogramEnabled

                });
            }
            catch (Exception e)
            {
                _logger.LogError("Couldn't send camera auto histogram via gRPC!");
            }

            return new Empty();
        }

        public void Dispose()
        {
            foreach(var v in _channels.Values)
                v.Dispose();
        }
    }
}
