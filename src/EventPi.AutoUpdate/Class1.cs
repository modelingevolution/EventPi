using Docker.DotNet;
using Docker.DotNet.Models;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Commands;
using Ductus.FluentDocker.Common;
using Ductus.FluentDocker.Services;
using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using System;
using System.Diagnostics.Metrics;
using System.Text.Json;

namespace EventPi.AutoUpdate
{
    public static class ContainerExtensions
    {
        public static IServiceCollection AddAutoUpdater(this IServiceCollection container)
        {
            container.AddSingleton<UpdateProcessManager>();
            container.AddSingleton<UpdateHost>();
            container.AddHostedService(sp => sp.GetRequiredService<UpdateHost>());
            return container;
        }

    }
    public class UpdateHost(IConfiguration config, ILogger<UpdateHost> log) : IHostedService
    {
        public IDictionary<string, string> Volumes { get; private set; }
        public ILogger Log => log;
        public static async Task<ContainerListResponse> GetContainer(string imageName = "modelingevolution/autoupdater")
        {
            using var config = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock"));
            using var client = config.CreateClient();
            var filters = new Dictionary<string, IDictionary<string, bool>> { { "status", new Dictionary<string, bool> { { "running", true } } } };

            var containers = await client.Containers.ListContainersAsync(new ContainersListParameters { Filters = filters });

            ContainerListResponse? c = containers.FirstOrDefault(c => c.Image.Contains(imageName));
            return c;
        }

        public static async Task<IDictionary<string, string>> GetVolumeMappings(string containerId)
        {

            using var config = new DockerClientConfiguration();
            using var client = config.CreateClient();
            var container = await client.Containers.InspectContainerAsync(containerId);
            var volumeMappings = new Dictionary<string, string>();

            if (container.HostConfig.Binds != null)
            {
                foreach (var bind in container.HostConfig.Binds)
                {
                    var parts = bind.Split(':');
                    if (parts.Length == 2)
                    {
                        var hostPath = parts[0];
                        var containerPath = parts[1];
                        volumeMappings[hostPath] = containerPath;
                    } 
                    else if(parts.Length > 2)
                    {
                        if (parts[0].Length == 1)
                        {
                            // it's most likely windows.
                            string hostPath = parts[0] + ":" + parts[1];
                            var containerPath = parts[2];
                            volumeMappings[hostPath] = containerPath;
                        }
                        else
                        {
                            var hostPath = parts[0];
                            var containerPath = parts[1];
                            volumeMappings[hostPath] = containerPath;
                        }
                    }
                }
            }

            return volumeMappings;

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            this.Volumes = await GetVolumeMappings((await GetContainer()).ID);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        //const string HostAddress = "host.docker.internal";
        const string HostAddress = "172.17.0.1";

        //const string HostAddress = "pi-200";
        internal async Task InvokeSsh(string command, string dockerComposeFolder, Action? onConnected = null)
        {
            var usr = config.GetValue<string>("SshUser") ?? throw new ArgumentException("Ssh user cannot be null");
            var pwd = config.GetValue<string>("SshPwd") ?? throw new ArgumentException("Ssh password cannot be null");
            using (var client = new SshClient(HostAddress, usr, pwd))
            {
                client.ServerIdentificationReceived += (s, e) => e.ToSuccess();
                client.HostKeyReceived += (sender, e) =>
                {
                    e.CanTrust = true;
                    log.LogInformation("SSH.HOST TRUSTED.");
                };
                client.Connect();
                onConnected?.Invoke();
                using SshCommand cmd = client.RunCommand(command);
                log.LogInformation($"Ssh: {command}, results: {cmd.Result}");

            }
        }
    }
    public readonly record struct GitTagVersion(string FriendlyName, System.Version Version) : IComparable<GitTagVersion> 
    {
        public static implicit operator string(GitTagVersion v)
        {
            return v.ToString();
        }
        public static GitTagVersion Parse(string text)
        {
            var v =  System.Version.Parse(text.Replace("ver","").Replace("v",""));
            return new GitTagVersion(text, v);
        }

        public int CompareTo(GitTagVersion other)
        {
            return this.Version.CompareTo(other.Version);
        }
        public override string ToString()
        {
            return FriendlyName;
        }
    }
    public record DeploymentState(string Version, DateTime Updated) { }
    public record DockerComposeConfiguration(string RepositoryLocation, string RepositoryUrl, string DockerComposeDirectory = "./")
    {
        public string ComposeFolderPath => Path.Combine(RepositoryLocation, DockerComposeDirectory);
        public string MergerName { get; init; } = "pi-admin";
        public string MergerEmail { get; init; } = "admin@eventpi.com";
        public string FriendlyName => Path.GetFileName(RepositoryLocation);
        public string? CurrentVersion
        {
            get
            {
                string stateFile = Path.Combine(ComposeFolderPath, "deployment.state.json");
                if (File.Exists(stateFile))
                    return JsonSerializer.Deserialize<DeploymentState>(File.ReadAllText(stateFile))?.Version;
                return null;
            }
        }
        public bool IsGitVersioned => Directory.Exists(RepositoryLocation) && Directory.Exists(Path.Combine(this.RepositoryLocation, ".git"));
        public bool CloneRepository()
        {
            if (!IsGitVersioned)
            {
                if (!Directory.Exists(RepositoryLocation))
                    Directory.CreateDirectory(RepositoryLocation);

                Repository.Clone(RepositoryUrl, RepositoryLocation);
                return true;
            }
            return false;
        }

        public IEnumerable<GitTagVersion> Versions()
        {
            if (!IsGitVersioned)
                CloneRepository();

            using var repo = new Repository(RepositoryLocation);
            var refSpecs = repo.Network.Remotes["origin"].FetchRefSpecs.Select(spec => spec.Specification);

            // Set up the fetch options
            var fetchOptions = new FetchOptions{};

            Commands.Fetch(repo, "origin", refSpecs, fetchOptions,null);
            foreach (var i in repo.Tags)
                yield return GitTagVersion.Parse(i.FriendlyName);
        }
        public bool Pull()
        {
            if (!IsGitVersioned)
            {
                this.CloneRepository();
                return true;
            }
            using var repo = new Repository(RepositoryLocation);
            var signature = new Signature(MergerName, MergerEmail, DateTimeOffset.Now);

            var pullOptions = new PullOptions()
            {
                FetchOptions = new FetchOptions() { },
                MergeOptions = new MergeOptions() { }
            };
            var result = Commands.Pull(repo, signature, pullOptions);
            return result.Status != MergeStatus.UpToDate;
        }

        public void Checkout(GitTagVersion version)
        {
            if (!IsGitVersioned)
            {
                this.CloneRepository();
            }
            using var repo = new Repository(RepositoryLocation);
            Tag tag = repo.Tags[version];

            if (tag == null)
                throw new Exception($"Tag {version} was not found.");
            

            // Checkout the tag
            CheckoutOptions options = new CheckoutOptions
            {
                CheckoutModifiers = CheckoutModifiers.None,
                CheckoutNotifyFlags = CheckoutNotifyFlags.None,
            };

            Commands.Checkout(repo, tag.Target.Sha, options);
        }


        private string? GetHostDockerComposeFolder(string pathInContainer, IDictionary<string, string> volumeMapping)
        {
            foreach(var v in volumeMapping)
            {
                if(pathInContainer.StartsWith(v.Value))
                    return pathInContainer.Replace(v.Value, v.Key);
            }
            return null;
        }
        public async Task Update(UpdateHost host)
        {
            var latest = this.Versions().OrderByDescending(x=>x.Version).FirstOrDefault();
            if (CurrentVersion != null && CurrentVersion == latest)
                return;

            Checkout(latest);
            
            // we need to find update container in docker and examine volume mappings.
            string dockerComposeFolder = GetHostDockerComposeFolder(ComposeFolderPath, host.Volumes) ?? throw new Exception("Cannot find path");

            DateTime n = DateTime.Now;
            string logFile = $"docker_compose_up_d_{n.Year}{n.Month}{n.Day}_{n.Hour}{n.Minute}{n.Second}.{n.Millisecond}.log";

            
            await host.InvokeSsh($"nohup docker compose up -d > {logFile} 2>&1 &", dockerComposeFolder, () =>
            {
                DeploymentState st = new DeploymentState(latest, n);
                string stateFile = Path.Combine(ComposeFolderPath, "deployment.state.json");
                File.WriteAllText(stateFile, JsonSerializer.Serialize(st));

            });
        }
        /// <summary>
        /// Performs update using direct to docker communication.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> InlineUpdate()
        {
            string? composeDir = ComposeFolderPath;
            if (Directory.Exists(composeDir))
            {
                var file = Directory.GetFiles(composeDir, "*.yml");
                
                using var svc = new Builder()
                    .UseContainer()
                    .UseCompose()
                    .FromFile(file)
                    .RemoveOrphans()
                    .Build();
                
                svc.Start();
                return true;
            }
            else
            {
                return false;
            }



        }
    }

    public class UpdateProcessManager(IConfiguration configuration, UpdateHost host, ILogger<UpdateProcessManager> logger)
    {

        public IEnumerable<DockerComposeConfiguration> GetPackages() => configuration.GetSection("Packages").Get<DockerComposeConfiguration[]>() ?? Array.Empty<DockerComposeConfiguration>();
        public async Task UpdateAll()
        {
            foreach(var i in GetPackages())
            {
                await i.Update(host);
            }
        }
    }
}
