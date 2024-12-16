using System.Diagnostics;
using System.Text;
using EventPi.Pid;
using EventPi.Pwm.Ui;
using FluentAssertions;
using Xunit.Abstractions;

namespace EventPi.Tests;

public class PidServiceTests(ITestOutputHelper _out)
{
    
    
    [Fact]
    public async Task PidService_Should_Converge_To_ExpectedValue()
    {
        // Arrange
        var c = new PidController(0.9,0,2,20,-20, 2);
        var pidService = new PidControllerTimeWrapper<PidController>(c);

        double processValue = 10;
        double setpoint = 50;
        
        var model = new StepMotorModel(1.8, 38000, 0.5,10);

        double tolerance = 0.5; // Acceptable error margin in mm

        // Simulation parameters
        const int iterations = 1000;
        const int simulationIntervalMs = 20; // Time per step in milliseconds

        var stopwatch = Stopwatch.StartNew();
        StringBuilder sb = new StringBuilder();
        PeriodicTimer t = new PeriodicTimer(TimeSpan.FromMilliseconds(simulationIntervalMs));
        for (int i = 0; i < iterations; i++)
        {
            // Act
            double adjustment = pidService.Compute(setpoint, processValue, simulationIntervalMs);

            // Let's map this tho step motor parameters.
            double motorTarget = processValue + adjustment*2;
            model.MoveTo(motorTarget);

            // Log the process value for debugging purposes
            sb.AppendLine($"{stopwatch.ElapsedMilliseconds}: Setpoint: {setpoint:F0}, Process value: {processValue:F2}, Adjustment: {adjustment:F2}, Motor target: {motorTarget:F2}, PID/I:{c.IntegralSum:F2} Running: {model.IsRunning()}");
            await t.WaitForNextTickAsync();
            processValue = model.Position();
            
            
        }

        stopwatch.Stop();
        _out.WriteLine(sb.ToString());
        await File.WriteAllTextAsync("C:\\Users\\rafal\\Sources\\ME\\rocket-welder\\src\\Submodules\\EventPi\\src\\EventPi.Tests\\log.txt", sb.ToString());
        // Assert
        model.Position().Should().BeApproximately(processValue, tolerance,
            "the PID controller should adjust the arm position close to the setpoint over time");

        // Optional: Validate that it reached the target within a reasonable duration
        stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(30),
            "the PID controller should converge within a reasonable time frame");
    }
}