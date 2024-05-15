using System.ComponentModel.DataAnnotations;
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

    // Should be SecretString
    [Required(AllowEmptyStrings = false)]
    [Length(8, 64)]
    public string Password { get; set; }
}