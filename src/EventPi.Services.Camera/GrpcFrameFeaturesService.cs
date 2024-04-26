using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using EventPi.Services.Camera;

namespace EventPi.Services.Camera;

public class GrpcFrameFeaturesService : FrameFeatures.FrameFeaturesBase
{
    private readonly ILogger<GrpcFrameFeaturesService> _logger;

    public GrpcFrameFeaturesService(ILogger<GrpcFrameFeaturesService> logger)
    {
        _logger = logger;
    }
    public override async Task<Empty> Process(FrameFeaturesRequest request, ServerCallContext context)
    {
        return new Empty();
    }


}