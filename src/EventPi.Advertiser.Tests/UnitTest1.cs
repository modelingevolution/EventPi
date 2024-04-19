using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace EventPi.Advertiser.Tests
{
    public class UnitTest1(ITestOutputHelper logger)
    {
        private TestAppHost _host = new TestAppHost(logger);


        [Fact]
        public void Test1()
        {

            _host.Configure(x => x.AddAdvertiser( new ServiceInfo("Video", 9001), new ServiceInfo("EventStore", 2113)));


        }

    }

}