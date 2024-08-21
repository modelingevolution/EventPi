using Docker.DotNet;
using Docker.DotNet.Models;
using Ductus.FluentDocker.Commands;
using Ductus.FluentDocker.Common;
using Ductus.FluentDocker.Extensions;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using System;
using System.Diagnostics.Metrics;

namespace EventPi.AutoUpdate
{
    public static class ContainerExtensions
    {
        public static IServiceCollection AddAutoUpdater(this IServiceCollection container)
        {
            container.AddSingleton<UpdateProcessManager>();
            container.AddSingleton<UpdateHost>();
            container.AddSingleton<DockerComposeConfigurationRepository>();
            container.AddHostedService(sp => sp.GetRequiredService<UpdateHost>());
            return container;
        }

    }
    public readonly record struct ContainerLogs(string Output, string Err);
    public record DockerRegistryPat(string Registry, string Base64);
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
        private static async Task<ContainerLogs> GetContainerLogs(string containerId)
        {
            using var config = new DockerClientConfiguration();
            using var client = config.CreateClient();

            var parameters = new ContainerLogsParameters
            {
                ShowStdout = true,
                ShowStderr = true,
                Follow = false,
                Tail = "all"
            };

            using (var stream = await client.Containers.GetContainerLogsAsync(containerId,true, parameters, CancellationToken.None))
            {
                var (o,e) = await stream.ReadOutputToEndAsync(CancellationToken.None);
                return new ContainerLogs(o, e);
            }
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
            try
            {
                var cid = (await GetContainer())?.ID;
                if (cid != null)
                {
                    this.Volumes = await GetVolumeMappings(cid);
                    log.LogInformation($"Docker volume mapping configured [{this.Volumes.Count}].");
                }
                else
                    log.LogInformation("Docker volume mapping is disabled.");
                await InvokeSsh("echo \"Hello\";");
            }
            catch (Exception ex) {
                log.LogError(ex, $"Cannot start {nameof(UpdateHost)}.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        //const string HostAddress = "host.docker.internal";
        const string HostAddress = "172.17.0.1";

        //const string HostAddress = "pi-200";
        internal async Task<String> InvokeSsh(string command, string? dockerComposeFolder = null, Action? onConnected = null)
        {
            var usr = config.GetValue<string>("SshUser") ?? throw new ArgumentException("Ssh user cannot be null");
            var pwd = config.GetValue<string>("SshPwd") ?? throw new ArgumentException("Ssh password cannot be null");
            using (var client = new SshClient(HostAddress, usr, pwd))
            {
                
                client.ServerIdentificationReceived += (s, e) => e.ToSuccess();
                client.HostKeyReceived += (sender, e) => {
                    e.CanTrust = true;
                    log.LogInformation("SSH.HOST TRUSTED.");
                };
                client.Connect();
                
                onConnected?.Invoke();

                if (dockerComposeFolder != null)
                    command = $"cd {dockerComposeFolder}; " + command;
                using SshCommand cmd = client.RunCommand(command);
                
                log.LogInformation($"Ssh: {command}, results: {cmd.Result}");
                return cmd.Result;

            }
        }
    }
    public record GitTagVersion(string FriendlyName, System.Version Version) : IComparable<GitTagVersion> 
    {
        public static implicit operator string(GitTagVersion v)
        {
            return v.ToString();
        }
        public static GitTagVersion? Parse(string? text)
        {
            if (text == null) return null;
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
    public class ContainerInfo2 : IContainerInfo
    {
        private DockerComposeConfiguration dockerComposeConfiguration;

        private string _id;
        public ContainerInfo2(DockerComposeConfiguration dockerComposeConfiguration, string v, string iD)
        {
            this.dockerComposeConfiguration = dockerComposeConfiguration;
            this.Name = v;
            this._id = iD;
        }

        public string Name { get; }

        public IList<string> Logs()
        {
            throw new NotImplementedException();
        }
    }
    public class ContainerInfo : IContainerInfo
    {
        private readonly IContainerService _container;
        public DockerComposeConfiguration Parent { get; }
        public string Name { get; }

        public ContainerInfo(DockerComposeConfiguration Configuration, string Name, IContainerService i)
        {
            this._container = i;
            this.Parent = Configuration;
            this.Name = Name;
        }

        public IList<string> Logs()
        {
            return _container.Logs().ReadToEnd();
        }
    }
    public record DeploymentState(string Version, DateTime Updated) { }
    public class DockerComposeConfigurationRepository
    {
        readonly DockerComposeConfiguration[] _items;
        private readonly IConfiguration configuration;

        public DockerComposeConfigurationRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
            _items = configuration.GetSection("Packages").Get<DockerComposeConfiguration[]>() ?? Array.Empty<DockerComposeConfiguration>();
        }

        public IEnumerable<DockerComposeConfiguration> GetPackages() => _items;

    }
    public class UpdateProcessManager(DockerComposeConfigurationRepository repo, UpdateHost host, ILogger<UpdateProcessManager> logger)
    {        
        public async Task UpdateAll()
        {
            foreach(var i in repo.GetPackages())
            {
                await i.Update(host);
            }
        }
    }
}
