using System.Net.NetworkInformation;
using Makaretu.Dns;

namespace EventPi.Advertiser;

public static class RpiAdvertiseTools
{
    public static string GetInterfaceAddress(NetworkInterface ni)
    {
        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
        {
            if (ip.Address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork) continue;
            if (ip.Address.ToString() != string.Empty) return ip.Address.ToString();
        }
        return string.Empty;
    }
    public static string GetOwnEthernetInterfaceAddress()
    {
        var interfaceEthernet = string.Empty;
        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if(ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet && ni.OperationalStatus==OperationalStatus.Up && ni.Name=="Ethernet")
            {
                interfaceEthernet = GetInterfaceAddress(ni);
            }
            //for Rpi
            switch (ni.Description)
            {
                case "eth0":
                    interfaceEthernet = GetInterfaceAddress(ni);
                    break;
            }
        }
        return interfaceEthernet;
    }
    public static string GetOwnWifiInterfaceAddress()
    {
        var interfaceWifi = string.Empty;
        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            switch (ni.Description)
            {
                case "wlan0":
                    interfaceWifi = GetInterfaceAddress(ni);
                    break;
            }
        }
        return interfaceWifi;

    }
    public static void GetWifiAndEthernet(IReadOnlyList<IReadOnlyDictionary<string, string>> properties, out string wifiAddress, out string ethernetAddress)
    {
        wifiAddress = String.Empty;
        ethernetAddress = String.Empty;
        foreach (var pair in properties)
        {
            if (pair.ContainsKey("Ethernet"))
            {
                ethernetAddress = pair["Ethernet"];
            }

            if (pair.ContainsKey("Wifi"))
            {
                wifiAddress = pair["Wifi"];
            }
        }
    }
    public static ServiceProfile AddWifiAndEthernetAddressesToProfile(this ServiceProfile profile)
    {
        profile.AddProperty("Wifi", GetOwnWifiInterfaceAddress());
        profile.AddProperty("Ethernet", GetOwnEthernetInterfaceAddress());
        return profile;
    }
}