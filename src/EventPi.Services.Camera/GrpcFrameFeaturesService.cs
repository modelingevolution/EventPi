using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using EventPi.Services.Camera;
using System;
using MicroPlumberd.DirectConnect;

namespace EventPi.Services.Camera;

//TODO: zrobic komende do kamery  oraz state kamery na UI - AppendState na plumberdzie, RangeValidator do komendy w atrybucie 

public class GrpcFrameFeaturesService : FrameFeatures.FrameFeaturesBase
{
    private readonly ILogger<GrpcFrameFeaturesService> _logger;

    public event EventHandler<FrameFeaturesRecord> OnFrameFeaturesAppeared;

    public GrpcFrameFeaturesService(ILogger<GrpcFrameFeaturesService> logger)
    {
        _logger = logger;
    }
    public override async Task<Empty> Process(FrameFeaturesRequest request, ServerCallContext context)
    {
        var ev = new FrameFeaturesRecord()
        {
            LargestSharedArea = request.LargestSharedArea,
            LargestSharedAreaHeight = request.LargestSharedAreaHeight,
            LargestSharedAreaWidth = request.LargestSharedAreaWidth,
            SharedAreasAmount = request.SharedAreasAmount,
            TotalSharedArea = request.TotalSharedArea,
            TotalBrightPixels = request.TotalBrightPixels,
            TotalDarkPixels = request.TotalDarkPixels,
        };
       // OnFrameFeaturesAppeared.Invoke(this,ev);
        _logger.LogInformation(ev.ToString());
        return new Empty();
    }


}