namespace EventPi.NetworkMonitor;

public enum WifiAccessPointMode
{
    //the device or access point mode is unknown
    Unkown = 0,
    //for both devices and access point objects, indicates the object is part of an Ad-Hoc 802.11 network without a central coordinating access point.    
    AdHoc = 1,
    //the device or access point is in infrastructure mode. For devices, this indicates the device is an 802.11 client/station. For access point objects, this indicates the object is an access point that provides connectivity to clients.
    Infrastructure = 2,
    //the device is an access point/hotspot. Not valid for access point objects; used only for hotspot mode on the local machine.
    AP = 3
}