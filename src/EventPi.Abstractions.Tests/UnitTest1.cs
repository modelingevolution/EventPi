using System.Text.Json;
using FluentAssertions;

namespace EventPi.Abstractions.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void One()
        {
            string cameraAddress = "Pi-51";
            var a= CameraAddress.Parse(cameraAddress,null);
            a.ToString().Should().Be(cameraAddress);
        }
        [Fact]
        public void Two()
        {
            string cameraAddress = "Pi-51/1";
            var a = CameraAddress.Parse(cameraAddress, null);
            a.ToString().Should().Be(cameraAddress);
            a.CameraNumber.Should().Be(1);
        }
    }
}