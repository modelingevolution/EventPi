using System.Runtime.InteropServices;

namespace EventPi.Threading;

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