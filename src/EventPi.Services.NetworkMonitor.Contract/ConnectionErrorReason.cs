namespace EventPi.Services.NetworkMonitor.Contract;

public enum ConnectionErrorReason
{
    Unknown,
    MissingProfile,
    AccessPointNotFound,
    LoginFailed,
    NoSecrets,
    IpConfigInvalid,
    ConnectTimeout,
    DeviceDisconnected
}