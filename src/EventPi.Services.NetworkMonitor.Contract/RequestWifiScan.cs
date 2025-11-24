using MicroPlumberd;
using MicroPlumberd.Services;

namespace EventPi.Services.NetworkMonitor.Contract;

[ThrowsFaultException<WrongHostError>]
public record RequestWifiScan
{

}