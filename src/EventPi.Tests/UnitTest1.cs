using System.Net;
using System.Net.WebSockets;
using Castle.Core.Logging;
using EventPi.SignalProcessing;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace EventPi.Tests
{
    public class SignalHubIntegrationTests : IAsyncLifetime
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly SignalHubServer _server;
        private readonly SignalHubClient _client;
        private readonly SignalMetadataFactory _metadataFactory;
        private readonly HttpListener _listener;
        private Task _serverTask;
        private const int TestPort = 8080;

        public SignalHubIntegrationTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _server = new SignalHubServer(NSubstitute.Substitute.For<ILogger<SignalHubServer>>());
            _server
                .RegisterSink<float>("temperature")
                .RegisterSink<float>("pressure");
            
            _client = new SignalHubClient(NSubstitute.Substitute.For < IHttpClientFactory>());
            _metadataFactory = new SignalMetadataFactory();
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{TestPort}/");
        }

        public async Task InitializeAsync()
        {
            // Start the WebSocket server
            _serverTask = Task.Run(StartWebSocketServer);
            await Task.Delay(100); // Give the server a moment to start
        }

        public async Task DisposeAsync()
        {
            _listener.Stop();
            await _serverTask;
        }

        private async Task StartWebSocketServer()
        {
            _listener.Start();
            while (_listener.IsListening)
            {
                HttpListenerContext context = await _listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
                    _ = HandleWebSocketConnection(webSocketContext.WebSocket, context.Request);
                }
            }
        }

        private async Task HandleWebSocketConnection(WebSocket webSocket, HttpListenerRequest request)
        {
            // Parse signals from query parameters
            var signalsParam = request.QueryString.GetValues("signals");
            var frequencyParam = request.QueryString["frequency"];

            // Create signal metadata
            var signals = new List<ISignalMetadata>();
            if (signalsParam != null) 
                signals.AddRange(signalsParam.Select(signalName => _server.GetMetadata(signalName)));

            // Use the SignalHubServer to pipe signals
            float frequency = float.TryParse(frequencyParam, out var freq) ? freq : 1f;
            await _server.PipeSignals(
                webSocket,
                frequency,
                CancellationToken.None,
                signals.ToArray()
            );
        }

        [Fact]
        public async Task SignalHub_EndToEnd_Integration_Test()
        {
            // Create sinks for sending signals
            var temperatureSink = _server.GetSink<float>("temperature");
            var pressureSink = _server.GetSink<float>("pressure");

            // Act
            // Subscribe to signals
            var receiverClient = await _client.Subscribe(
                _metadataFactory.Create<float>("temperature"),
                _metadataFactory.Create<float>("pressure")
            );
            receiverClient.Start();

            await Task.Delay(10000);
            _testOutputHelper.WriteLine("Writing signals...");
            // Send some test signals
            temperatureSink.Write(25.5f);
            pressureSink.Write(1013.25f);

            // Collect received signals
            var receivedSignals = new List<IDictionary<ushort, object>>();

            _ = Task.Run(async () =>
                {
                    await foreach (var signals in receiverClient.ReadAll())
                        receivedSignals.Add(signals.Values);
                }
            );

        // Wait for signals to be processed
            await Task.Delay(50000);

            // Assert
            Assert.NotEmpty(receivedSignals);
            Assert.All(receivedSignals, signals =>
            {
                Assert.Contains((ushort)0, signals); // temperature
                Assert.Contains((ushort)1, signals); // pressure
                Assert.Equal(25.5f, signals[0]);
                Assert.Equal(1013.25f, signals[1]);
            });
        }
    }
}