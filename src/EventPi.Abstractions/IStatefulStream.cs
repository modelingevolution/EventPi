namespace EventPi.Abstractions;

public interface IStatefulStream<in TIdentifier>
{
    static abstract string FullStreamName(TIdentifier id);
}