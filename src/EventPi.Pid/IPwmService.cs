namespace EventPi.Pid
{
    public interface IPwmService
    {
        bool IsReverse { get; set; }
        bool IsRunning { get; }
        void Start();
        void Stop();
        void ChangeToForward();
        void ChangeToBackward();
        int Frequency { get; set; }
        double DutyCycle { get; set; }
    }
}
