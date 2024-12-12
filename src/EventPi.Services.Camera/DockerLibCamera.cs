using System.Net;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;

namespace EventPi.Services.Camera;

public enum ContainerStatus
{
    DoesNotExist,
    Stopped,
    Running
}

public class DockerLibCamera(ILogger<DockerLibCamera> logger, string image, string tag, string pat) : ILibCameraVidProcess
{
    private const string App = "/usr/local/bin/rocketwelder-vid";
    public string ImageName = $"{image}:{tag}";
    public async Task Stop(int camera)
    {
        string containerName = $"vid-{camera}";
        using DockerClient client = new DockerClientConfiguration().CreateClient();
        var status = await client.Check(containerName);
        if (status == ContainerStatus.Running) 
            await client.Containers.StopContainerAsync(containerName, new ContainerStopParameters { WaitBeforeKillSeconds = 5 });
    }
    
    public async Task<int> Start(Resolution resolution, VideoCodec codec, string tuningFilePath, VideoTransport transport,
        IPAddress? listenAddress = null, int listenPort = 6000, string grpcListenAddress = "127.0.0.1:6500",
        string shmName = "default", int? cameraNr = null)
    {
        string containerName = $"vid-{(cameraNr ?? 0)}";
        var args = LibCameraVidProcess.Args(resolution, codec, tuningFilePath, transport, listenAddress, listenPort, grpcListenAddress, shmName, cameraNr);
        var command = App;
        var fullCmd = args.Prepend(command).ToArray();
        
        using DockerClient client = new DockerClientConfiguration().CreateClient();

        var status = await client.Check(containerName);

        var containerConfig = new CreateContainerParameters
        {
            Image = ImageName,
            Name = containerName,
            
            HostConfig = new HostConfig
            {
                Privileged = true,
                NetworkMode = "host",
                Binds = new List<string>
                {
                    "/dev:/dev",
                    "/run/udev:/run/udev:ro",
                    "/var/run/dbus/system_bus_socket:/var/run/dbus/system_bus_socket",
                    "/home/pi/imx296.json:/app/imx296.json",
                },
                
                LogConfig = new LogConfig
                {
                    Type = "json-file",
                    Config = new Dictionary<string, string>
                    {
                        { "max-size", "25m" },
                        { "max-file", "10" }
                    }
                }
            },
            User = "root", // Should be only when debugging.
            Entrypoint = fullCmd,
            Env = new List<string>
            {
                "UDEV=1"
            }
        };
        
        logger.LogInformation($"docker run " +
                              "--entrypoint {1}" +
                              "-v /dev:/dev " +
                              "-v /run/udev:/run/udev:ro " +
                              "-v /var/run/dbus/system_bus_socket:/var/run/dbus/system_bus_socket " +
                              "-v /home/pi/imx296.json:/app/imx296.json " +
                              "--log-opt max-size=25m --log-opt max-file=10 " +
                              "-e UDEV=1 --name {0} {2}",
                            containerName, ImageName, string.Join(" ", fullCmd));
        if ((status == ContainerStatus.Running || status == ContainerStatus.Stopped) &&
            (await client.GetImageName(containerName)) != ImageName)
        {
            var prv = await client.GetImageName(containerName);
            logger.LogInformation($"Version of the image has changed, from {prv} to {ImageName}");
            if(status == ContainerStatus.Running)
            {
                logger.LogInformation("Stopping old container.");
                await client.Containers.StopContainerAsync(containerName,
                    new ContainerStopParameters { WaitBeforeKillSeconds = 5 });
                
            }
            logger.LogInformation("Removing old container with all volumes.");
            await client.Containers.RemoveContainerAsync(containerName, new ContainerRemoveParameters());
            status = ContainerStatus.DoesNotExist;
        }
        
        if (status == ContainerStatus.DoesNotExist || (await client.GetImageName(containerName)) != ImageName)
        {
            var authConfig = new AuthConfig
            {
                ServerAddress = "docker.io",
                Username = "modelingevolution",           
                Password = pat
            };
            logger.LogInformation($"Pulling new image: {ImageName}");
            await client.Images.CreateImageAsync(
                new ImagesCreateParameters { FromImage = image, Tag = tag },
                authConfig,
                new Progress<JSONMessage>(m => Console.WriteLine(m.Status))
            );
            logger.LogInformation($"Creating new container: {containerName}");
            var response = await client.Containers.CreateContainerAsync(containerConfig);
            if (response.Warnings != null && response.Warnings.Count > 0)
            {
                Console.WriteLine("Warnings:");
                foreach (var warning in response.Warnings)
                {
                    Console.WriteLine(warning);
                }
            }

            status = ContainerStatus.Stopped;
        }

        if (status == ContainerStatus.Running)
        {
            await client.Containers.StopContainerAsync(containerName,
                new ContainerStopParameters { WaitBeforeKillSeconds = 5 });
            await Task.Delay(1000);
        }
        
        bool started = await client.Containers.StartContainerAsync(containerName, new ContainerStartParameters());
        Console.WriteLine(started
            ? $"Container '{containerName}' started successfully."
            : $"Failed to start container '{containerName}'.");
        if (started)
        {
            var state = await client.Containers.InspectContainerAsync(containerName);
            return (int)state.State.Pid;
        }

        return -1;
    }
}