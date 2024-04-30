using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using WeldingAutomation.CameraOptions;
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
        public async Task<Empty> ProcessAsync(CameraConfigurationProfile ev)
        {
            
            return new Empty();
        }
        public async Task<Empty> ProcessAsync(ICameraParameters ev)
        {
            var client = new CameraOptions.CameraOptionsClient(_toCppChannel);
            Empty? response = await client.ProcessAsync(new CameraOptionsRequest()
            {
                AnologueGain = ev.AnalogueGain,
                BlueGain = ev.BlueGain,
                RedGain = ev.RedGain,
                Brightness = ev.Brightness,
                Contrast = ev.Contrast,
                DigitalGain = ev.DigitalGain,
                Sharpness = ev.Sharpness
            });
            return new Empty();
        }

        public void Dispose()
        {
            _toCppChannel.Dispose();
        }
    }
}
