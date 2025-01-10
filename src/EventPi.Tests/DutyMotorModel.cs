using EventPi.Pwm.Ui;
using Xunit.Abstractions;

namespace EventPi.Tests;
using Xunit;
using NSubstitute;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using EventPi.Pid;
using Microsoft.Extensions.Logging;

public class StepMotorControllerSpecs 
{
    private readonly ITestOutputHelper _output;

    private readonly IPwmService _pwmMock;
    private readonly ILogger<StepMotorController> _loggerMock;
    private readonly ILoggerFactory loggerFactory;

    public StepMotorControllerSpecs(ITestOutputHelper output)
    {
        this._output = output;

        _pwmMock = Substitute.For<IPwmService>();
        this.loggerFactory = LoggerFactory.Create(builder => builder.AddXunit(output));
        _loggerMock = loggerFactory.CreateLogger<StepMotorController>();

    }

    [Fact]
    public void WhenConstructed_ShouldInitializeProperly()
    {
        // Given
        // No preconditions

        // When
        using var controller = CreateSut();
        // Then
        controller.Should().NotBeNull();
        _pwmMock.Received(0).Start(); // PWM should not start immediately
    }

    private StepMotorController CreateSut()
    {
        StepMotorController? controller = null;
        try
        {
            controller = new StepMotorController(_pwmMock, _loggerMock);
            _output.WriteLine(controller.MotorModel.ToString());
            return controller;
        }
        catch
        {
            controller?.Dispose();
            throw;
        }
    }

    [Fact]
    public async Task WhenMoveToCalled_ShouldSendCommandsToMotor()
    {
        // Given
        var targetPosition = 100.0;
        using var controller = CreateSut();


        // When
        controller.MoveTo(targetPosition);

        // Simulate the internal task processing the command
        await Task.Delay(200); // Allow time for the background task to process

        // Then
        _pwmMock.Received(1).Start();
        _pwmMock.IsReverse.Should().BeFalse(); // Assumes positive direction
    }

    [Fact]
    public async Task WhenMoveToCalledWithNegativeValue_ShouldReverseMotor()
    {
        // Given
        var targetPosition = -50.0;
        using var controller = CreateSut();

        // When
        controller.MoveTo(targetPosition);

        // Simulate the internal task processing the command
        await Task.Delay(200); // Allow time for the background task to process

        // Then
        _pwmMock.Received(1).Start();
        _pwmMock.IsReverse.Should().BeTrue(); // Assumes negative direction
    }

    [Fact]
    public async Task WhenDisposed_ShouldStopMotorAndReleaseResources()
    {
        // Given
        using var controller = CreateSut();

        controller.MoveTo(100);
        // When
        controller.Dispose();
        
        await Task.Delay(200);
        // Then
        _pwmMock.Received(1).Stop();
    }
    [Fact]
    public async Task WhenTimeElaspes_ShouldStopMotorCheckTime()
    {
        // Given
        var pwm = new NullPwmService(loggerFactory.CreateLogger<NullPwmService>());
        using var controller = CreateSut();

        controller.MoveTo(100);
        // When
        await Task.Delay(3000);

        // Then
        pwm.IsRunning.Should().BeFalse();
    }
    [Fact]
    public async Task WhenTimeElaspes_ShouldStopMotor()
    {
        // Given        using var controller = CreateSut();
        var controller = CreateSut();
        controller.MoveTo(10);
        // When
        await Task.Delay(3000);

        // Then
        _pwmMock.Received(1).Stop();
    }

    [Fact]
    public async Task WhenMultipleCommandsSent_ShouldProcessLatestCommand()
    {
        // Given
        var initialPosition = 10.0;
        var finalPosition = 200.0;
        using var controller = CreateSut();

        // When
        controller.MoveTo(initialPosition);
        controller.MoveTo(finalPosition);

        // Simulate the internal task processing the commands
        await Task.Delay(200); // Allow time for the background task to process

        // Then
        _pwmMock.Received(1).Start();
        // Additional logic can be verified with mocks if required
    }
}
public class DutyMotorModel
{
    private readonly double _maxRpm;
    private readonly double _mmPerRotation;
    private double _Position;
    private double _motorRpm;
    private double _dutyCycle;
    public DutyMotorModel(double maxRpm, double mmPerRotation, double initialPosition = 0)
    {
        _maxRpm = maxRpm;
        _mmPerRotation = mmPerRotation;
        _Position = initialPosition;
        _motorRpm = 0;
    }

    public double DutyCycle => _dutyCycle;
    public double Position => _Position;

    public void Run(double dutyCycle, long milliseconds)
    {
        Run(dutyCycle, TimeSpan.FromMilliseconds(milliseconds));
    }
    public void Run(double dutyCycle, TimeSpan dt)
    {
        // Clamp duty cycle between 0 and 1
        _dutyCycle = Math.Clamp(dutyCycle, -0.5, 0.5);

        // Simulate motor RPM based on duty cycle
        _motorRpm = _maxRpm * _dutyCycle * 2;

        // Calculate arm position change based on motor RPM and time step
        double rotations = _motorRpm * dt.TotalSeconds; // Convert RPM to rotations per time step
        _Position += rotations * _mmPerRotation; // Update arm position
    }
}