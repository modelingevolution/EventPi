using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EventPi.Services.NetworkMonitor.Contract;
using MicroPlumberd.Encryption;
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

    class DefineWirelessProfileSurrogate
    {
        public string? FileName { get; set; }
        public string InterfaceName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MinLength(1)]
        public string Ssid { get; set; }

        // Should be SecretString
        [Required(AllowEmptyStrings = false)]
        [Length(8, 64)]
        public string Password { get; set; }

        public DefineWirelessProfile Command(string recipient)
        {
            return new DefineWirelessProfile()
            {
                FileName = FileName,
                InterfaceName = InterfaceName,
                Password = SecretObject<string>.Create(Password, recipient),
                Ssid = Ssid
            };
        }
    }
}
