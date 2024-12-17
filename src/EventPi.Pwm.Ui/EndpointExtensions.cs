using System.Globalization;
using System.Net.WebSockets;
using EventPi.SignalProcessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Patterns;

namespace EventPi.Pwm.Ui;

public static class EndpointExtensions
{
    public static void MapSignals(this IEndpointRouteBuilder b)
    {
        b.MapGet("/signals", Handlers.Get);
        b.Map("/signals-stream", Handlers.GetStream);
    }
}


public static class Handlers
{
    public static IEnumerable<SignalSink> Get([FromServices] SignalHubServer srv)
    {
        foreach (var m in srv.MetadataIndex.OrderBy(x=>x.Name))
            yield return new SignalSink(m.Name, m.Type.Name);
    }

    public static async Task GetStream(HttpContext context, [FromServices] SignalHubServer srv)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var query = context.Request.Query;

            if (!query.TryGetValue("signals", out var signalNames))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var signals = signalNames.Select(name => srv.GetMetadata(name)).ToArray();

            if (!float.TryParse(query["frequency"], CultureInfo.InvariantCulture, out var frequency)) frequency = 1.0f; // Default frequency

            var cancellationToken = context.RequestAborted;
            await srv.PipeSignals(webSocket, frequency, cancellationToken, signals);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}