using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using EventPi.Services.Camera;
using System;
using MicroPlumberd;
using EventPi.Abstractions;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace EventPi.Services.Camera;

public class GrpcFrameFeaturesService : FrameFeatures.FrameFeaturesBase
{
    public event EventHandler<FrameFeaturesRecord> OnFrameFeaturesAppeared;
    public GrpcFrameFeaturesService()
    {
     
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
            Lux = request.Lux,

        };
        OnFrameFeaturesAppeared.Invoke(this,ev);
      
        return new Empty();
    }


}