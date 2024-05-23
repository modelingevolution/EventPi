using Microsoft.Extensions.Configuration;

namespace EventPi.Abstractions;

class HostEnvironment(IConfiguration config) : IEnvironment
{
    private readonly HostName _host = (HostName)(config.GetValue<string>("HostName") ?? Environment.MachineName);

    public HostName HostName => _host;

}