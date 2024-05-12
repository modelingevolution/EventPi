using EventPi.Abstractions;
using MicroPlumberd;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventPi.Services.NetworkMonitor.Contract
{
    public class WrongHostError
    {
        public static readonly WrongHostError Error = new();
    }
    
}
