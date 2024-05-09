namespace EventPi.NetworkMonitor
{


    public enum DeviceType
    {
        Unknown = 0,
        Ethernet = 1,
        Wifi = 2,
        Unused1 = 3,
        Unused2 = 4,
        BT = 5,
        OLPCMesh = 6,
        Wimax = 7,
        Modem = 8,
        Infiniband = 9,
        Bond = 10,
        VLAN = 11,
        ADSL = 12,
        Bridge = 13,
        Generic = 14,
        Team = 15,
        TUN = 16,
        IPTunnel = 17,
        MACVLAN = 18,
        VXLAN = 19,
        VETH = 20,
        MACsec = 21,
        Dummy = 22,
        PPP = 23,
        OVSInterface = 24,
        OVSPort = 25,
        OVSBridge = 26,
        WPAN = 27,
        Lowpan = 28,
        WireGuard = 29,
        WiFiP2P = 30
    }
    public enum NetworkManagerState : uint
    {
        Unknown = 0,
        ASleep = 10,
        Disconnected = 20,
        Disconnecting = 30,
        Connecting = 40,
        ConnectedLocal = 50,
        ConnectedSite = 60,
        ConnectedGlobal = 70
    }
    public enum NetworkManagerConnectivity : uint
    {
        Unknown = 0,
        None = 1,
        Portal = 2,
        Limited = 3,
        Full = 4
    }
}
