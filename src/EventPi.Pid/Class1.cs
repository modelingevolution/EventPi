using System.Diagnostics;

namespace EventPi.Pid
{
    /// <summary>
    /// Ziegler-Nichols Method:
    /// This method provides initial estimates for the gains based on the system’s response.
    /// Start by setting (K_i) and (K_d) to zero.
    /// Increase (K_p) until you observe sustained oscillations (system becomes marginally stable).
    /// Note the critical gain value ((K_{pc})) and the corresponding period ((T_{pc})).
    /// Use the following relationships:
    /// (K_p = 0.6 \cdot K_{pc})
    /// (T_i = 0.5 \cdot T_{pc})
    /// (T_d = 0.125 \cdot T_{pc})
    /// </summary>
    public class PidController
    {
        private double _kp; // Proportional Gain
        private double _ki; // Integral Gain
        private double _kd; // Derivative Gain
        
        private double _outputUpperLimit; // Controller Upper Output Limit
        private double _outputLowerLimit; // Controller Lower Output Limit
        private double integralSum;
        private double prevError;
        private readonly Stopwatch _sw;
        public PidController(double kp, double ki, double kd,  double outputUpperLimit, double outputLowerLimit)
        {
            _kp = kp;
            _ki = ki;
            _kd = kd;
            
            _outputUpperLimit = outputUpperLimit;
            _outputLowerLimit = outputLowerLimit;
            integralSum = 0;
            prevError = 0;
            _sw = new Stopwatch();
            _sw.Start();
        }

        public double CalculateOutput(double setPoint, double processValue)
        {
            var ret = CalculateOutput(setPoint, processValue, _sw.Elapsed);
            _sw.Restart();
            return ret;
        }
        public double CalculateOutput(double setPoint, double processValue, TimeSpan ts)
        {
            double error = setPoint - processValue;
            integralSum += error * ts.TotalSeconds;
            double derivative = (error - prevError) / ts.TotalSeconds;
            double output = _kp * error + _ki * integralSum + _kd * derivative;
            output = Math.Max(_outputLowerLimit, Math.Min(_outputUpperLimit, output)); // Clamp output within limits
            prevError = error;
            return output;
        }

        public void ResetController()
        {
            integralSum = 0;
            prevError = 0;
            _sw.Restart();
        }
    }
}
