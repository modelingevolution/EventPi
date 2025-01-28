namespace EventPi.Pid;

public interface IPidConfig
{
    /// <summary>
    /// Proportional Gain, consider resetting controller if this parameter is drastically changed.
    /// </summary>
    public double Kp { get; set; }

    /// <summary>
    /// Integral Gain, consider resetting controller if this parameter is drastically changed.
    /// </summary>
    public double Ki { get; set; }

    /// <summary>
    /// Derivative Gain, consider resetting controller if this parameter is drastically changed.
    /// </summary>
    public double Kd { get; set; }
    /// <summary>
    /// Upper output limit of the controller.
    /// This should obviously be a numerically greater value than the lower output limit.
    /// </summary>
    public double OutputUpperLimit { get; set; }

    /// <summary>
    /// Lower output limit of the controller
    /// This should obviously be a numerically lesser value than the upper output limit.
    /// </summary>
    public double OutputLowerLimit { get; set; }

    public double? IntegralErrorThreshold { get; set; }
}