namespace EventPi.Abstractions;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now { get => DateTime.Now;  }
    
}