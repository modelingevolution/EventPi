﻿using System.Net.NetworkInformation;
using Makaretu.Dns;

namespace EventPi.Advertiser;

public static class DiscoveryProperties
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
            // A very dirty heuristics.
            if(ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet && ni.OperationalStatus==OperationalStatus.Up && ni.Name.Contains("Ethernet"))
            {
                if(ni.Description.Contains("Hyper-V")) 
                    continue;
                interfaceEthernet = GetInterfaceAddress(ni);
                break;
            }
            //for Rpi
            if (ni.Description == "eth0")
            {
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
            if (ni.Description == "wlan0")
            {
                interfaceWifi = GetInterfaceAddress(ni);
                break;
            }
            else if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 &&
                     ni.OperationalStatus == OperationalStatus.Up)
            {
                interfaceWifi = GetInterfaceAddress(ni);
                break;
            }
        }
        return interfaceWifi;

    }
    public static void RetriveProperties(IReadOnlyList<IReadOnlyDictionary<string, string>> properties, out string wifiAddress, out string ethernetAddress, out string schema)
    {
        wifiAddress = String.Empty;
        ethernetAddress = String.Empty;
        schema = "tcp";
        foreach (var dict in properties)
        {
            if (dict.ContainsKey("Ethernet"))
            {
                ethernetAddress = dict["Ethernet"];
            }

            if (dict.ContainsKey("Wifi"))
            {
                wifiAddress = dict["Wifi"];
            }

            if (dict.ContainsKey("Schema"))
                schema = dict["Schema"];
        }
        
    }
    public static ServiceProfile AddWifiAndEthernetAddressesToProfile(this ServiceProfile profile)
    {
        profile.AddProperty("Wifi", GetOwnWifiInterfaceAddress());
        profile.AddProperty("Ethernet", GetOwnEthernetInterfaceAddress());
        return profile;
    }
}