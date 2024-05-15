namespace EventPi.Services.NetworkMonitor.Contract;

public class ConnectionError
{
    public string Message { get; init; }
    public ConnectionErrorReason Reason { get; init; }
    public string ProfileFileName { get; init; }
}