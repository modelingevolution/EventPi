using System.Runtime.InteropServices.Marshalling;

namespace EventPi.NetworkMonitor;

public enum ActivationState : uint
{
    Unknown = 0,
    Activating = 1,
    Activated = 2,
    Deactivating = 3,
    Deactivated = 4
}

public enum ActivationFailedReason : uint
{
    /// <summary>
    /// The nm active connection state reason unknownThe reason for the active connection state change is unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The nm active connection state reason noneNo reason was given for the active connection state change.
    /// </summary>
    None = 1,

    /// <summary>
    /// The nm active connection state reason user disconnected/The active connection changed state because the user disconnected it.
    /// </summary>
    UserDisconnected = 2,

    /// <summary>
    /// The nm active connection state reason device disconnectedThe active connection changed state because the device it was using was disconnected.
    /// </summary>
    DeviceDisconnected = 3,

    /// <summary>
    /// The nm active connection state reason service stoppedThe service providing the VPN connection was stopped.
    /// </summary>
    ServiceStopped = 4,

    /// <summary>
    /// The nm active connection state reason ip configuration invalidThe IP config of the active connection was invalid.
    /// </summary>
    IpConfigInvalid = 5,



    /// <summary>
    /// The nm active connection state reason connect timeoutThe connection attempt to the VPN service timed out.
    /// </summary>
    ConnectTimeout = 6,




    /// <summary>
    /// The nm active connection state reason service start timeoutA timeout occurred while starting the service providing the VPN connection.
    /// </summary>
    ServiceStartTimeout = 7,



    /// <summary>
    /// The nm active connection state reason service start failedStarting the service providing the VPN connection failed.
    /// </summary>
    ServiceStartFailed = 8,




    /// <summary>
    /// The nm active connection state reason no secretsNecessary secrets for the connection were not provided.
    /// </summary>
    NoSecrets = 9,



    /// <summary>
    /// The nm active connection state reason login failed
    /// </summary>
    LoginFailed = 10,



    /// <summary>
    /// The nm active connection state reason connection removed
    /// </summary>
    ConnectionRemoved = 11,



    /// <summary>
    /// The nm active connection state reason dependency failed
    /// </summary>
    DependencyFailed = 12,




    /// <summary>
    /// The nm active connection state reason device realize failed
    /// </summary>
    DeviceRealizeFailed = 13,



    /// <summary>
    /// The nm active connection state reason device removed
    /// </summary>
    DeviceRemoved = 14
}

public enum DeviceState : uint
{
    /// <summary>
    /// the device's state is unknown
    /// </summary>
    Unknown = 0,
    /// <summary>
    /// the device is recognized, but not managed by NetworkManager
    /// </summary>
    Unmanaged = 10,
    /// <summary>
    /// the device is managed by NetworkManager, but is not available for use. Reasons may include the wireless switched off, missing firmware, no ethernet carrier, missing supplicant or modem manager, etc.
    /// </summary>
    Unavailable = 20,
    /// <summary>
    /// the device can be activated, but is currently idle and not connected to a network.
    /// </summary>
    Disconnected = 30,

    /// <summary>
    /// the device is preparing the connection to the network. This may include operations like changing the MAC address, setting physical link properties, and anything else required to connect to the requested network.
    /// </summary>
    Prepare = 40,
    /// <summary>
    /// the device is connecting to the requested network. This may include operations like associating with the WiFi AP, dialing the modem, connecting to the remote Bluetooth device, etc.
    /// </summary>
    Config = 50,

    /// <summary>
    /// the device requires more information to continue connecting to the requested network. This includes secrets like WiFi passphrases, login passwords, PIN codes, etc.
    /// </summary>
    NeedAuth = 60,
    /// <summary>
    /// the device is requesting IPv4 and/or IPv6 addresses and routing information from the network.
    /// </summary>
    IpConfig = 70,
    /// <summary>
    /// the device is checking whether further action is required for the requested network connection. This may include checking whether only local network access is available, whether a captive portal is blocking access to the Internet, etc.
    /// </summary>
    IpCheck = 80,

    /// <summary>
    /// the device is waiting for a secondary connection (like a VPN) which must activated before the device can be activated
    /// </summary>
    Secondaries = 90,

    /// <summary>
    /// the device has a network connection, either local or global.
    /// </summary>
    Activated = 100,

    /// <summary>
    /// a disconnection from the current network connection was requested, and the device is cleaning up resources used for that connection. The network connection may still be valid.
    /// </summary>
    Deactivating = 110,

    /// <summary>
    /// the device failed to connect to the requested network and is cleaning up the connection request
    /// </summary>
    Failed = 120,
}