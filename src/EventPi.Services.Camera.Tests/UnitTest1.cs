using MicroPlumberd;
using MicroPlumberd.Services;

using System.Diagnostics;
using EventStore.Client;
using Xunit.Abstractions;
using FluentAssertions;
using ModelingEvolution.DirectConnect;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using BoDi;
using MicroPlumberd.Services;
using Xunit.Abstractions;
using TechTalk.SpecFlow;

namespace EventPi.Services.Camera.Tests
{
    public class CommandHandlerTests : IClassFixture<EventStoreServer>
    {
        private readonly EventStoreServer _eventStore;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly TestAppHost _serverTestApp;
        private readonly TestAppHost _clientTestApp;

        public CommandHandlerTests(EventStoreServer eventStore, ITestOutputHelper testOutputHelper)
        {
            _eventStore = eventStore;
            _serverTestApp = new TestAppHost(testOutputHelper);
           
        }
       
        [Fact]
        public async Task HandleCommand()
        {
            await _eventStore.StartInDocker();

            _serverTestApp.Configure(x => x
                .AddPlumberd(_eventStore.GetEventStoreSettings())
                .AddCommandHandler<CameraProfileConfigurationCommandHandler>(start: StreamPosition.Start));

            var srv = await _serverTestApp.StartAsync();

            var cmd = new CreateCameraConfiguration() { Hostname = "Hello" };
            var recipientId = Guid.NewGuid();

            await srv.GetRequiredService<ICommandBus>().SendAsync(recipientId, cmd);

           
            var fooModel = new FooModel(new InMemoryModelStore());
            var sub = await srv.GetRequiredService<IPlumber>().SubscribeEventHandler(fooModel);
            await Task.Delay(1000);

            fooModel.ModelStore.Index.Should().HaveCount(1);
            fooModel.ModelStore.Index[0].Metadata.CausationId().Should().Be(cmd.Id);
            fooModel.ModelStore.Index[0].Metadata.CorrelationId().Should().Be(cmd.Id);
            fooModel.ModelStore.Index[0].Metadata.SourceStreamId.Should().Be($"CameraConfigurationAggregate-{recipientId}");
        }

       
    }
}