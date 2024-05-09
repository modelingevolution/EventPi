

using ModelingEvolution.DirectConnect;

namespace EventPi.Abstractions;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now { get => DateTime.Now;  }
    
}
public interface IEnvironment
{
    HostName HostName { get; }
}

public class HostEnvironment : IEnvironment
{
    public HostName HostName => (HostName)Environment.MachineName;
}