using FluentAssertions;
using Xunit.Abstractions;

namespace EventPi.Pid.Tests
{
    public class PidTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public PidTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Same()
        {
            double actual = 200;
            double expected = 200;


            var pid = new PidController(1, 0, 0,500,-500);

            pid.CalculateOutput(expected, expected, TimeSpan.FromSeconds(1));
            var d = pid.CalculateOutput(expected, actual, TimeSpan.FromSeconds(1));
            d.Should().Be(0);
        }
        [Fact]
        public void Lower()
        {
            double actual = 201;
            double expected = 200;


            var pid = new PidController(1, 0, 0, 500, -500);

            pid.CalculateOutput(expected, expected, TimeSpan.FromSeconds(1));
            var d = pid.CalculateOutput(expected, actual, TimeSpan.FromSeconds(1));
            d.Should().BeLessThan(0);
            _testOutputHelper.WriteLine("We need to change shutter by: " + d.ToString());
        }
        [Theory]
        [InlineData(1,0,0, 200, 200)]
        [InlineData(1, 0, 0, 200, 250)]
        [InlineData(1, 0, 0, 200, 150)]

        [InlineData(1, 1, 0, 200, 250)]
        [InlineData(1, 1, 0, 200, 150)]
        public void Samples(double p, double d, double i, double actual, double expected)
        {
            var pid = new PidController(p, d, i, 500, -500);

            pid.CalculateOutput(expected, expected, TimeSpan.FromSeconds(1));
            var o = pid.CalculateOutput(expected, actual, TimeSpan.FromSeconds(1));
            
            _testOutputHelper.WriteLine($"{actual} should be {expected}, we need to change shutter by: {o}");
        }
    }
}