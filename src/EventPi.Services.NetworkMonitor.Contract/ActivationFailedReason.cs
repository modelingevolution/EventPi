namespace EventPi.Services.NetworkMonitor.Contract;

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
    DeviceRemoved = 14,
}