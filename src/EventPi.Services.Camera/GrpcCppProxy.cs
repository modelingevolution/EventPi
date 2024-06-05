using EventPi.Services.Camera.Contract;
using EventPi.Services.CameraAutoHistogram;
using EventPi.Services.CameraAutoShutter;
using EventPi.Services.CameraConfigurator;
using EventPi.Services.CameraOptions;
using EventPi.Services.CameraShutter;
using EventPi.Services.CameraGreeter;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using static System.Net.WebRequestMethods;

namespace EventPi.Services.Camera
{
    public class GrpcCppCameraProxy : IDisposable
    {
        private GrpcChannel _toCppChannel;
        private readonly ILogger<GrpcCppCameraProxy> _logger;
        private string _cppGrpcUri = "";

        
        public GrpcCppCameraProxy(ILogger<GrpcCppCameraProxy> logger, IConfiguration config)
        {
            _logger = logger;
            var grpcUrl = config["RpiCam" + ":GrpcReceiverAddress"]?? string.Empty;
            InitProxy(grpcUrl);
        }

        public void InitProxy(string url="")
        {
            //_cppGrpcUri = string.IsNullOrWhiteSpace(url)? "http://192.168.0.105:6500" : url;
            _cppGrpcUri = string.IsNullOrWhiteSpace(url)? "http://127.0.0.1:6500" : url;
            _logger.LogInformation($"Grpc cpp proxy initialized for address: {_cppGrpcUri}");
            _toCppChannel = GrpcChannel.ForAddress(_cppGrpcUri);
        }

        public async Task<Empty> ProcessAsync(CameraAutoShutterRequest ev)
        {
            var client = new CameraAutoShutter.CameraAutoShutter.CameraAutoShutterClient(_toCppChannel);
            try
            {
                await client.ProcessAsync(ev);
            }
            catch (Exception e)
            {
                _logger.LogError("Couldn't set CameraAutoShutterRequest via gRPC!");
            }
            return new Empty();
        }
        public async Task<Empty> ProcessAsync(SetCameraHistogramFilter ev)
        {
            var client = new CameraConfigurator.CameraConfigurator.CameraConfiguratorClient(_toCppChannel);

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
        public async Task<bool> GreetWithRpiCam()
        {
            try
            {
                var clientOptions = new CameraGreeter.CameraGreeter.CameraGreeterClient(_toCppChannel);
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
        public async Task<Empty> ProcessAsync(CameraConfigurationProfile ev)
        {
            
            return new Empty();
        }
        public async Task<Empty> ProcessAsync(ICameraParameters ev)
        {
            _logger.LogInformation("Trying to set parameters to camera...");

            try
            {
                var clientOptions = new CameraOptions.CameraOptions.CameraOptionsClient(_toCppChannel);
                await clientOptions.ProcessAsync(new CameraOptionsRequest()
                {
                    AnologueGain = ev.AnalogueGain,
                    BlueGain = ev.BlueGain,
                    RedGain = ev.RedGain,
                    Brightness = ev.Brightness,
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
                var clientShutter = new CameraShutter.CameraShutter.CameraShutterClient(_toCppChannel);
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
                    new CameraAutoHistogram.CameraAutoHistogram.CameraAutoHistogramClient(_toCppChannel);
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
            _toCppChannel.Dispose();
        }
    }
}
