


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace EventPi.Abstractions;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now { get => DateTime.Now;  }
    
}

public static class ContainerExtensions
{
    public static IServiceCollection AddEventPiAbstractions(this IServiceCollection services) => 
        services.AddSingleton<IEnvironment, HostEnvironment>();
}
public interface IEnvironment
{
    HostName HostName { get; }
}

public class HostEnvironment(IConfiguration config) : IEnvironment
{
    private readonly HostName _host = (HostName)(config.GetValue<string>("HostName") ?? Environment.MachineName);

    public HostName HostName => _host;

}