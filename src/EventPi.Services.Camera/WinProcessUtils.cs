namespace EventPi.Services.Camera;


#if WINDOWS
using System.Managemenet;
class WinProcessUtils : IProcessUtils
{
    public int GetParentProcessId(int pid)
    {
        int parentProcessId = 0;

        // Use WMI to query the Win32_Process class for the ParentProcessId
        string query = $"SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {processId}";
        using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
        {
            using (ManagementObjectCollection results = searcher.Get())
            {
                foreach (ManagementObject mo in results)
                {
                    parentProcessId = Convert.ToInt32(mo["ParentProcessId"]);
                    break;
                }
            }
        }

        return parentProcessId;
    }
}
#endif