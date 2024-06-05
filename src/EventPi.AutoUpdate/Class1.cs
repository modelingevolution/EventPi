using Ductus.FluentDocker.Builders;
using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EventPi.AutoUpdate
{
    public class UpdateProcessManager(IConfiguration configuration, ILogger<UpdateProcessManager> logger)
    {

        public async Task<bool> IsNewVersionAvailable()
        {
            string composeDir = configuration.GetValue<string>("ComposeDir");
            string merger = configuration.GetValue<string>("MergerName");
            string mergeEmail = configuration.GetValue<string>("MergerEmail");
            if (Directory.Exists(composeDir))
            {
                using var repo = new Repository(composeDir);
                var signature = new Signature(merger ?? "admin", mergeEmail ?? "admin@admin", DateTimeOffset.Now);

                var pullOptions = new PullOptions()
                {
                    FetchOptions = new FetchOptions() { }, 
                    MergeOptions = new MergeOptions() { }
                };
                var result = Commands.Pull(repo, signature, pullOptions);
                if(result.Status == MergeStatus.UpToDate)
                {
                    logger.LogInformation("No new version available");
                    return false;
                }
                else
                {
                    logger.LogInformation("New version available");
                    return true;
                }
            }
            else
                logger.LogError("Compose directory not found: {0}", composeDir);
            return false;
        }

        public async Task Prepare()
        {
            // Docker compose pull
        }
        public async Task Update()
        {
            string composeDir = configuration.GetValue<string>("ComposeDir");
            if (Directory.Exists(composeDir))
            {
                var file = Directory.GetFiles(composeDir, "*.yml");
                logger.LogInformation("About to perform an update: {0}", composeDir);
                using var svc = new Builder()
                    .UseContainer()
                    .UseCompose()
                    .FromFile(file)
                    .RemoveOrphans()
                    .Build();
                
                svc.Start();
                logger.LogInformation("Update completed.");
            }
            else
            {
                logger.LogError("Compose directory not found: {0}", composeDir);
            }

           

        }
    }
}
