namespace EventPi.Abstractions;

public readonly record struct Ip4Config(Ip4 Ip, uint Prefix, Ip4 Gateway)
{
    public override string ToString()
    {
        return $"{Ip}/{Prefix}, gw. {Gateway}";
    }
}