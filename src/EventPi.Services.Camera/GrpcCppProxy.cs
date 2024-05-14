using EventPi.Services.Camera.Contract;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using WeldingAutomation.CameraConfigurator;
using WeldingAutomation.CameraOptions;
using WeldingAutomation.CameraShutter;
using static System.Net.WebRequestMethods;

namespace EventPi.Services.Camera
{
 
    public class GrpcCppCameraProxy : IDisposable
    {
        private GrpcChannel _toCppChannel;
        private readonly ILogger<GrpcCppCameraProxy> _logger;
        private string _cppGrpcUri = "";

        public GrpcCppCameraProxy(ILogger<GrpcCppCameraProxy> logger)
        {
            _logger = logger;
            InitProxy();
        }

        public void InitProxy(string url="")
        {
            _cppGrpcUri = string.IsNullOrWhiteSpace(url)? "http://127.0.0.1:6500" : url;
            _logger.LogInformation($"Grpc cpp proxy initialized for address: {_cppGrpcUri}");
            _toCppChannel = GrpcChannel.ForAddress(_cppGrpcUri);
        }

        public async Task<Empty> ProcessAsync(SetCameraHistogramFilter ev)
        {
            var client = new CameraConfigurator.CameraConfiguratorClient(_toCppChannel);
         
            await client.ProcessAsync(new ConfigureHistogramRequest()
            {
                ConfigurationName = ev.Id.ToString(),
                Histogram =  ByteString.CopyFrom(ev.Values)

            });
        return new Empty();
        }
        public async Task<Empty> ProcessAsync(CameraConfigurationProfile ev)
        {
            
            return new Empty();
        }
        public async Task<Empty> ProcessAsync(ICameraParameters ev)
        {
            _logger.LogInformation("Trying to set parameters to camera...");
          var clientOptions = new CameraOptions.CameraOptionsClient(_toCppChannel);
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
          var clientShutter = new CameraShutter.CameraShutterClient(_toCppChannel);
            await clientShutter.ProcessAsync(new ConfigureShutterRequest()
           {
             Shutter = ev.Shutter,
             ExposureLevel = ev.ExposureLevel

           });
            return new Empty();
        }

        public void Dispose()
        {
            _toCppChannel.Dispose();
        }
    }
}
