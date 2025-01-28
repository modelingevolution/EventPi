using FluentAssertions;
using System;
using Xunit;
using EventPi.Abstractions;

namespace EventPi.Abstractions.Tests
{
    public class CameraAddressTests
    {
        [Fact]
        public void One()
        {
            string cameraAddress = "Pi-51";
            var a= CameraAddress.Parse(cameraAddress,null);
            a.ToString().Should().Be(cameraAddress);
            a.CameraNumber.Should().BeNull();
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