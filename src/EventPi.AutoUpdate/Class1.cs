using Microsoft.Extensions.Configuration;

namespace EventPi.AutoUpdate
{
    public class UpdateProcessManager(IConfiguration configuration)
    {

        public async Task<bool> IsNewVersionAvailable()
        {
            return false;
        }
        public async Task Update()
        {
            
        }
    }
}
