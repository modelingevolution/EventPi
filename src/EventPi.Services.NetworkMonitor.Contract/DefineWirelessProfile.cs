using System.ComponentModel.DataAnnotations;
using MicroPlumberd.Encryption;
using MicroPlumberd.Services;

namespace EventPi.Services.NetworkMonitor.Contract;

[ThrowsFaultException<WrongHostError>]
[ThrowsFaultException<ConnectionError>]
public record DefineWirelessProfile 
{
    public string? FileName { get; set; }
    public string InterfaceName { get; set; }

    [Required(AllowEmptyStrings = false)]
    [MinLength(1)]
    public string Ssid { get; set; }

    public SecretObject<string> Password { get; set; }
}

