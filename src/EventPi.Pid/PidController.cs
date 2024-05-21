using System.Runtime.CompilerServices;

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
    public class PidController : IPidConfig, IController
    {
        private double _kp; // Proportional Gain
        private double _ki; // Integral Gain
        private double _kd; // Derivative Gain
        
        private double _outputUpperLimit; // Controller Upper Output Limit
        private double _outputLowerLimit; // Controller Lower Output Limit
        private double _integralSum;
        private double _prevError;
        
        public PidController(double kp,  double kd, double ki, double outputUpperLimit, double outputLowerLimit)
        {
            Kp = kp;
            Ki = ki;
            Kd = kd;
            
            OutputUpperLimit = outputUpperLimit;
            OutputLowerLimit = outputLowerLimit;
            _integralSum = 0;
            _prevError = 0;
           
        }

        public double Kp
        {
            get => _kp;
            set => _kp = value;
        }

        public double Ki
        {
            get => _ki;
            set => _ki = value;
        }

        public double Kd
        {
            get => _kd;
            set => _kd = value;
        }

        public double OutputUpperLimit
        {
            get => _outputUpperLimit;
            set => _outputUpperLimit = value;
        }

        public double OutputLowerLimit
        {
            get => _outputLowerLimit;
            set => _outputLowerLimit = value;
        }

       
        public double CalculateOutput(double setPoint, double processValue, TimeSpan ts)
        {
            double error = setPoint - processValue;
            _integralSum += error * ts.TotalSeconds;
            double derivative = (error - _prevError) / ts.TotalSeconds;
            double output = Kp * error + Ki * _integralSum + Kd * derivative;
            output = Math.Max(OutputLowerLimit, Math.Min(OutputUpperLimit, output)); // Clamp output within limits
            _prevError = error;
            return output;
        }

        public void ResetController()
        {
            _integralSum = 0;
            _prevError = 0;
        }
    }
}
