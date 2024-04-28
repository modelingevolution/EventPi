namespace EventPi;

interface IServiceInfo
{
    string Schema { get; }
    string ServiceName { get; }
    int Port { get; }
};