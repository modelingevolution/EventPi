using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using static System.Net.WebRequestMethods;

namespace EventPi.Services.Camera
{
 
    public class GrpcCppProxy : IDisposable
    {
        private GrpcChannel _toCppChannel;
        private readonly ILogger<GrpcCppProxy> _logger;
        private string _cppGrpcUri = "";

        public GrpcCppProxy(ILogger<GrpcCppProxy> logger)
        {
            _logger = logger;
        }

        public void InitProxy(string url="")
        {
            _cppGrpcUri = string.IsNullOrWhiteSpace(url)? "http://localhost:6500" : url;
            _logger.LogInformation($"Grpc cpp proxy initialized for address: {_cppGrpcUri}");
            _toCppChannel = GrpcChannel.ForAddress(_cppGrpcUri);
        }
        public async Task<Empty> ProcessAsync(CameraConfigurationProfile ev)
        {
      
            return new Empty();
        }

        public void Dispose()
        {
            _toCppChannel.Dispose();
        }
    }
}
