using System.Net.Http.Json;
using System.Text.Json;
using AutoBanners.Models;
using AutoBanners.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace AutoBanners.Services;

public class PortainerHealthService : HealthServiceBase, IHealthService<PortainerConfiguration>
{
    protected override string ServiceName => "Portainer";
    
    public PortainerHealthService(ILogger<PortainerHealthService> logger, HttpClient client)
        : base(logger, client) { }

    public Task<HealthStatus> GetHealthAsync(PortainerConfiguration configuration)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            Headers =
            {
                { "X-API-Key", configuration.ApiKey }
            },
            RequestUri = new Uri($"{configuration.BaseAddress.AbsoluteUri.TrimEnd('/')}/api/endpoints/{configuration.EnvironmentId}/docker/containers/{configuration.ContainerName}/json")
        };

        return CheckHealthAsync(request, async response =>
        {
            var containerInfo =
                await response.Content.ReadFromJsonAsync<ContainerInfo>();

            return containerInfo?.State.Health.Status == "healthy"
                ? HealthStatus.Healthy
                : HealthStatus.Unhealthy;
        });
    }
}

class ContainerInfo
{
    public ContainerState State { get; set; } = new();
}

class ContainerState
{
    public ContainerHealth Health { get; set; } = new();
}

class ContainerHealth
{
    public string Status { get; set; } = string.Empty;
}