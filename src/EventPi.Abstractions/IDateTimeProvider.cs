namespace EventPi.Abstractions;

public interface IDateTimeProvider
{
    public DateTime Now { get; }
}