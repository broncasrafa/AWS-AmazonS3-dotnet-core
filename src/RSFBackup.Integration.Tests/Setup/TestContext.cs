using System.Runtime.InteropServices;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace RSFBackup.Integration.Tests.Setup;

public class TestContext : IAsyncLifetime
{
    private readonly DockerClient _dockerClient;
    private const string _ContainerImageUri = "localstack/localstack";
    private const string _Port = "9003";
    private string _ContainerId;


    public TestContext()
    {
        _dockerClient = new DockerClientConfiguration(new Uri(DockerApiUri())).CreateClient();
    }

    
    private string DockerApiUri()
    {
        bool isWindowsOS = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        if (isWindowsOS)
            return "npipe://./pipe/docker_engine";

        bool isLinuxOS = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        bool isMacOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        if (isLinuxOS || isMacOS)
            return "unix:/var/run/docker.sock";

        throw new Exception("Unable to determine what OS this is running on");
    }
    private async Task PullDockerImageAsync()
    {
        await _dockerClient.Images.CreateImageAsync(new ImagesCreateParameters
        {
            FromImage = _ContainerImageUri,
            Tag = "latest"
        },
        new AuthConfig(),
        new Progress<JSONMessage>());
    }
    private async Task StartDockerContainerAsync()
    {
        var response = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = _ContainerImageUri,
            ExposedPorts = new Dictionary<string, EmptyStruct>
            {
                { _Port, default }
            },
            HostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    { _Port, new List<PortBinding> { new PortBinding { HostPort = _Port } } }
                }
            },
            Env = new List<string>
            {
                $"SERVICES=s3:{_Port}"
            }
        });

        _ContainerId = response.ID;
        await _dockerClient.Containers.StartContainerAsync(_ContainerId, null);
    }


    public async Task InitializeAsync()
    {
        await PullDockerImageAsync();
        await StartDockerContainerAsync();
    }

    public async Task DisposeAsync()
    {
        if (_ContainerId != null)
            await _dockerClient.Containers.KillContainerAsync(_ContainerId, new ContainerKillParameters());
    }
}
