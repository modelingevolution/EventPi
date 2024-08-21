
namespace EventPi.AutoUpdate
{
    public interface IContainerInfo
    {
        string Name { get; }

        IList<string> Logs();
    }
}