namespace NetworkManager.DBus;

partial class NetworkManagerService
{
    internal Tmds.DBus.Protocol.Connection Connection { get; }
    internal string Destination { get; }
    internal NetworkManagerService(Tmds.DBus.Protocol.Connection connection, string destination)
        => (Connection, Destination) = (connection, destination);
    internal ObjectManager CreateObjectManager(string path) => new ObjectManager(this, path);
    internal NetworkManager CreateNetworkManager(string path) => new NetworkManager(this, path);
    internal IP4Config CreateIP4Config(string path) => new IP4Config(this, path);
    internal Active CreateActive(string path) => new Active(this, path);
    internal AgentManager CreateAgentManager(string path) => new AgentManager(this, path);
    internal Statistics CreateStatistics(string path) => new Statistics(this, path);
    internal Wireless CreateWireless(string path) => new Wireless(this, path);
    internal Device CreateDevice(string path) => new Device(this, path);
    internal WifiP2P CreateWifiP2P(string path) => new WifiP2P(this, path);
    internal Bridge CreateBridge(string path) => new Bridge(this, path);
    internal Loopback CreateLoopback(string path) => new Loopback(this, path);
    internal Veth CreateVeth(string path) => new Veth(this, path);
    internal Wired CreateWired(string path) => new Wired(this, path);
    internal Ppp CreatePpp(string path) => new Ppp(this, path);
    internal DnsManager CreateDnsManager(string path) => new DnsManager(this, path);
    internal AccessPoint CreateAccessPoint(string path) => new AccessPoint(this, path);
    internal IP6Config CreateIP6Config(string path) => new IP6Config(this, path);
    internal Settings CreateSettings(string path) => new Settings(this, path);
    internal Connection CreateConnection(string path) => new Connection(this, path);
}