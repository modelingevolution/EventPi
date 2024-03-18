namespace EventPi.Abstractions;

public interface ICloneable<out T>
{
    T Clone();
}