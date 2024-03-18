using ModelingEvolution.Plumberd;

namespace EventPi.Abstractions;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now { get => DateTime.Now;  }
    
}
public interface IEventStoreStream
{
    Task Write<T>(Guid id, T e) where T : IEvent;
}
public interface IEnvironment
{
    string DeviceSN { get; }
}
public readonly struct DeviceStreamSuffix : IEquatable<DeviceStreamSuffix>
{
    private readonly string _sn;
    private readonly DateOnly _date;
    private readonly Guid _id;

    public string Sn => this._sn;

    public Guid Id => this._id;

    public DateOnly Date => this._date;

    private DeviceStreamSuffix(string sn, DateOnly date, Guid id)
    {
        this._sn = sn;
        this._date = date;
        this._id = id;
    }

    public static DeviceStreamSuffix Create(string deviceSn, DateTime? n = null)
    {
        return DeviceStreamSuffix.Create(deviceSn ?? throw new ArgumentNullException("DeviceSn"), n ?? DateTime.Now);
    }

    public static DeviceStreamSuffix Create(string deviceSn, DateTime date)
    {
        long binary = date.Date.ToBinary();
        Guid id = deviceSn.ToLower().ToGuid().Combine((ulong)binary);
        return new DeviceStreamSuffix(deviceSn, DateOnly.FromDateTime(date), id);
    }

    public static implicit operator Guid(DeviceStreamSuffix s) => s.Id;

    public override string ToString() => this.Id.ToString();

    public bool Equals(DeviceStreamSuffix other) => this._id.Equals(other._id);

    public override bool Equals(object? obj)
    {
        return obj is DeviceStreamSuffix other && this.Equals(other);
    }

    public override int GetHashCode() => this._id.GetHashCode();

    public static bool operator ==(DeviceStreamSuffix left, DeviceStreamSuffix right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DeviceStreamSuffix left, DeviceStreamSuffix right)
    {
        return !left.Equals(right);
    }
}
public class HostEnvironment : IEnvironment
{
    public string DeviceSN => Environment.MachineName;
}