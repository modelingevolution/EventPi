using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EventPi.Services.Camera;


#if WINDOWS
[StructLayout(LayoutKind.Sequential)]
public struct PROCESS_BASIC_INFORMATION
{
    public IntPtr Reserved1;
    public IntPtr PebBaseAddress;
    public IntPtr Reserved2_0;
    public IntPtr Reserved2_1;
    public IntPtr UniqueProcessId;
    public IntPtr InheritedFromUniqueProcessId;
}


class WinProcessUtils : IProcessUtils
{
    [DllImport("ntdll.dll")]
    private static extern int NtQueryInformationProcess(
        IntPtr processHandle,
        int processInformationClass,
        ref PROCESS_BASIC_INFORMATION processInformation,
        uint processInformationLength,
        out uint returnLength);
    public int GetParentProcessId(int pid)
    {
        try
        {
            Process process = Process.GetProcessById(pid);
            IntPtr processHandle = process.Handle;

            PROCESS_BASIC_INFORMATION pbi = new PROCESS_BASIC_INFORMATION();
            uint returnLength;

            int status = NtQueryInformationProcess(processHandle, 0, ref pbi, (uint)Marshal.SizeOf(pbi), out returnLength);

            if (status != 0)
                throw new Exception("Unable to query process information.");

            return pbi.InheritedFromUniqueProcessId.ToInt32();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving parent process: {ex.Message}");
            return -1; // return -1 in case of failure
        }
    }
}
#endif