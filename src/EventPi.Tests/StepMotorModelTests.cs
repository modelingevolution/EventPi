using System;
using System.Threading.Tasks;
using EventPi.Pid;
using Xunit;
using FluentAssertions;
namespace EventPi.Tests;
public class StepMotorModelTests
{
    [Fact]
    public async Task MoveTo_ShouldStartMovement_WhenMotorIsIdle()
    {
        // Arrange
        var model = new StepMotorModel(1.8, 100, 10);
        double position = 5;
        // Act
        model.MoveTo(position);
        // Wait for the motor to "finish" its work
        await Task.Delay(1000); // Simulate 1 second of motor operation
        // Assert
        model.Position().Should().BeApproximately(5, 0.01); // Initial Position
        model.Position(DateTime.Now.AddSeconds(1)).Should().BeApproximately(5,0.01); // Movement started
    }
    [Fact]
    public async Task MoveTo_ShouldAdjustMovement_WhenMovingInSameDirection()
    {
        // Arrange
        var model = new StepMotorModel(1.8, 100, 10);
        double initialPosition = 10;
        model.MoveTo(initialPosition);
        // Wait for the motor to "finish" its initial movement
        await Task.Delay(1500);
        // Act
        double nextPosition = 15;
        model.MoveTo(nextPosition);
        // Wait for the motor to "finish" its additional movement
        await Task.Delay(1500);
        // Assert
        model.Position().Should().BeApproximately(15, 0.01);
    }
    [Fact]
    public async Task MoveTo_ShouldChangeDirection_WhenMovingInOppositeDirection()
    {
        // Arrange
        var model = new StepMotorModel(1.8, 100, 10);
        double initialPosition = 5;
        model.MoveTo(initialPosition);
        // Wait for the motor to "finish" its initial movement
        await Task.Delay(500);
        // Act
        double oppositePosition = -5;
        model.MoveTo(oppositePosition);
        // Wait for the motor to "finish" its opposite movement
        await Task.Delay(2000);
        // Assert
        model.Position().Should().BeApproximately(-5, 0.01); 
    }
   
}
