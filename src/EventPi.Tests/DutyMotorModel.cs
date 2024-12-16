namespace EventPi.Tests;

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