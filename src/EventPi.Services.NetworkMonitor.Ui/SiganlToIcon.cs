using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MudBlazor;

namespace EventPi.Services.NetworkMonitor.Ui
{
    internal static class SignalToIcon
    {
        public static string Convert(byte signal) =>
            signal switch
            {
                0 => Icons.Material.Filled.SignalWifiOff,
                < 15 => Icons.Material.Filled.SignalWifi0Bar,
                < 40 => Icons.Material.Filled.NetworkWifi1Bar,
                < 70 => Icons.Material.Filled.NetworkWifi2Bar,
                < 90 => Icons.Material.Filled.NetworkWifi3Bar,
                _ => Icons.Material.Filled.SignalWifi4Bar
            };
    }
}
