using System.Runtime.InteropServices;

namespace EventPi.Threading;
public class PrecisePeriodicTimer : IDisposable
{
    private readonly TimeSpan _interval;
    private volatile bool _isDisposed;
    private DateTime _nextScheduledTime;
    
    public PrecisePeriodicTimer(TimeSpan interval)
    {
        if (interval <= TimeSpan.Zero)
            throw new ArgumentException("Interval must be positive", nameof(interval));

        _interval = interval;
        _nextScheduledTime = DateTime.UtcNow + interval;
    }

    public void WaitForNextIteration()
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(PrecisePeriodicTimer));

        DateTime now = DateTime.UtcNow;

        // If we're past the scheduled time, update to next interval
        if (now >= _nextScheduledTime)
        {
            _nextScheduledTime = now + _interval;
            return;
        }

        // Calculate remaining time
        var remainingTime = _nextScheduledTime - now;

        // If remaining time is significant, sleep for most of it
        if (remainingTime > TimeSpan.FromMilliseconds(100))
        {
            // Sleep for (remainingTime - 100ms)
            Thread.Sleep((int)(remainingTime.TotalMilliseconds - 100));
        }

        // Recalculate remaining time and spin for precise waiting
        now = DateTime.UtcNow;
        remainingTime = _nextScheduledTime - now;

        // Use SpinWait for precise final waiting
        if (remainingTime > TimeSpan.Zero)
        {
            var spinWait = new SpinWait();
            var targetTime = DateTime.UtcNow + remainingTime;

            while (DateTime.UtcNow < targetTime)
            {
                spinWait.SpinOnce();
            }
        }

        // Update next scheduled time
        _nextScheduledTime += _interval;
    }

    public void Dispose()
    {
        _isDisposed = true;
    }
}
public static class Interop
{
    [DllImport("sem.so", SetLastError = true, EntryPoint = "open_sem")]
    public static extern IntPtr sem_open(string name, int oflag, uint mode, uint value);

    // Import the sem_wait function
    [DllImport("libc", SetLastError = true)]
    public static extern int sem_wait(IntPtr sem);

    // Import the sem_post function
    [DllImport("libc", SetLastError = true)]
    public static extern int sem_post(IntPtr sem);

    // Import the sem_close function
    [DllImport("libc", SetLastError = true)]
    public static extern int sem_close(IntPtr sem);

    // Import the sem_unlink function
    [DllImport("libc", SetLastError = true)]
    public static extern int sem_unlink(string name);

    [DllImport("libc")]
    public static extern int errno(); // Assuming this retrieves errno
}