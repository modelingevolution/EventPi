using System.Runtime.InteropServices;

namespace EventPi.Abstractions.IO;

/// <summary>
/// Provides Unix-specific file operations that work across platforms
/// </summary>
public class UnixFile
{
    /// <summary>
    /// Creates a hard link between the specified paths
    /// </summary>
    /// <param name="sourceFileName">The path of the file to link from</param>
    /// <param name="destFileName">The path where the hard link should be created</param>
    /// <exception cref="PlatformNotSupportedException">Thrown when hard links are not supported on the current platform</exception>
    /// <exception cref="FileNotFoundException">Thrown when the source file does not exist</exception>
    /// <exception cref="IOException">Thrown when the operation fails</exception>
    public static bool CreateHardLink(string sourceFileName, string destFileName)
    {
        if (string.IsNullOrEmpty(sourceFileName))
            throw new ArgumentNullException(nameof(sourceFileName));

        if (string.IsNullOrEmpty(destFileName))
            throw new ArgumentNullException(nameof(destFileName));

        if (!File.Exists(sourceFileName))
            throw new FileNotFoundException("Source file not found.", sourceFileName);

        if (File.Exists(destFileName))
            return false;

        // Use platform-specific implementation
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            CreateHardLinkWindows(sourceFileName, destFileName);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                 RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            CreateHardLinkUnix(sourceFileName, destFileName);
           
        }
        else
        {
            throw new PlatformNotSupportedException("Hard links are not supported on this platform.");
        }
        return true;
    }

    // Windows implementation using P/Invoke
    private static void CreateHardLinkWindows(string sourceFileName, string destFileName)
    {
        if (!CreateHardLink(destFileName, sourceFileName, IntPtr.Zero))
        {
            int error = Marshal.GetLastWin32Error();
            throw new IOException($"Failed to create hard link. Error code: {error}");
        }
    }

    // Unix/Linux/macOS implementation using P/Invoke
    private static void CreateHardLinkUnix(string sourceFileName, string destFileName)
    {
        int result = link(sourceFileName, destFileName);
        if (result != 0)
        {
            int errno = Marshal.GetLastWin32Error();
            throw new IOException($"Failed to create hard link. Error code: {errno}");
        }
    }

    // Windows P/Invoke declaration
    [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CreateHardLink(
        string lpFileName,
        string lpExistingFileName,
        IntPtr lpSecurityAttributes);

    // Unix P/Invoke declaration
    [DllImport("libc", SetLastError = true)]
    private static extern int link(string oldpath, string newpath);
}