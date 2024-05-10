using System.Text;
using NetworkManager.DBus;
using Tmds.DBus.Protocol;
using Connection = NetworkManager.DBus.Connection;

namespace EventPi.NetworkMonitor;

public record WifiNetwork
{
        
    public string SourceInterface { get; init; }
    internal string DevicePath { get; init; }
    internal string AccessPointPath { get; init; }
    public string Ssid { get; init; }
    public byte SignalStrength { get; init; }
    public WifiAccessPointMode AccessPointMode { get; init; }
    public uint MaxKbitRate { get; init; }
    public DeviceInfo SourceDevice { get; set; }
    internal NetworkManagerClient Client { get; init; }
    public async Task<ConnectionInfo> Connect(string password)
    {
        var connectionSettings = new Dictionary<string, Dictionary<string, Variant>>
        {
            {
                "802-11-wireless", new Dictionary<string, Variant>
                {
                    //{ "ssid", Variant.FromArray<byte>(new Array<byte>(Encoding.UTF8.GetBytes(Ssid))) },
                    { "ssid", new Variant(Ssid)},
                    { "mode", new Variant("infrastructure") },
                    { "security", new Variant("802-11-wireless-security") }
                }
            },
            {
                "802-11-wireless-security", new Dictionary<string, Variant>
                {
                    { "psk", new Variant(password) },
                    { "auth-alg", new Variant("shared") },
                    { "key-mgmt", new Variant("wpa-psk") },
                    { "psk-flags", new Variant(0u)} // none, system is reposible for storing pwd
                }
            },
            {
                "connection", new Dictionary<string, Variant>
                {
                    { "type", new Variant("802-11-wireless") },
                    { "id", new Variant(Ssid) }
                }
            },
        };
        var result = await Client.NetworkManager.AddAndActivateConnectionAsync(connectionSettings, DevicePath, AccessPointPath);
        var connection = Client.Service.CreateConnection(result.Path);
        return new ConnectionInfo()
        {
            Path = result.Path,
            FileName =  await connection.GetFilenameAsync(),
            Client = Client
        };
    }
    
    internal Wireless Wireless
    {
        get => Client.Service.CreateWireless(this.DevicePath);
    }
    public async Task RequestScan(bool wait = true)
    {
        await Wireless.RequestScanAsync(new Dictionary<string, Variant>());
        if (wait) await Task.Delay(5000);
    }


    public async Task<ConnectionInfo> Setup(string pwd, string conName)
    {
        var connectionSettings = new Dictionary<string, Dictionary<string, Variant>>
        {
            {
                "802-11-wireless", new Dictionary<string, Variant>
                {
                    { "ssid", Variant.FromArray<byte>(new Array<byte>(Encoding.UTF8.GetBytes(Ssid))) },
                    //{ "ssid", new Variant(Ssid)},
                    //{ "mode", new Variant("infrastructure") },
                    //{ "security", new Variant("802-11-wireless-security") }
                }
            },
            {
                "802-11-wireless-security", new Dictionary<string, Variant>
                {
                    { "psk", new Variant(pwd) },
                    { "key-mgmt", new Variant("wpa-psk") },
                    { "psk-flags", new Variant(0u)} // none, system is reposible for storing pwd
                }
            },
            {
                "connection", new Dictionary<string, Variant>
                {
                    { "type", new Variant("802-11-wireless") },
                    { "id", new Variant(conName??Ssid) },
                    { "interface-name", new Variant(this.SourceInterface)},
                }
            },
        };
        var r = await Client.Settings.AddConnectionAsync(connectionSettings);
        var connection = Client.Service.CreateConnection(r);
        return new ConnectionInfo()
        {
            Path = r,
            FileName = await connection.GetFilenameAsync(),
            Client = Client
        };
    }
}

public record WifiSettings
{
    public string Ssid { get; init; }
    public string Mode { get; init; }
    public string Pwd { get; init; }
}
public record ConnectionInfo
{
    internal Connection Connection => Client.Service.CreateConnection(Path);
    internal string Path { get; init; }
    public string? FileName { get; init; }

    public async Task<WifiSettings?> WifiSettings()
    {
        var props = await Connection.GetSettingsAsync();
        if (props.TryGetValue("802-11-wireless", out var wifiSettings))
        {
            //var secrets = await Connection.GetSecretsAsync(null);

            string ssid = wifiSettings.TryGetValue("ssid", out var r) ? Encoding.UTF8.GetString(r.GetArray<byte>()) : null;
            string mode = wifiSettings.TryGetValue("mode", out var m) ? m.GetString() : null;
            return new WifiSettings() { Ssid = ssid, Mode = mode };
        }

        
        else return null;
    }
    internal NetworkManagerClient Client { get; init; }

    public async Task Delete()
    {
        await Connection.DeleteAsync();
    }

    public async Task UpdatePwd(string pwd)
    {

    }
}