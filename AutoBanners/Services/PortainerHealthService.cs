using System.Net.Http.Json;
using AutoBanners.Models;

namespace AutoBanners.Services;

public class PortainerHealthService : IHealthService<PortainerConfiguration>
{
    private readonly HttpClient _client;

    public PortainerHealthService(HttpClient client)
    {
        _client = client;
    }

    public async Task<HealthStatus> GetHealthAsync(PortainerConfiguration configuration)
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

        var response = await _client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return HealthStatus.Unhealthy;
        }

        var containerInfo = await response.Content.ReadFromJsonAsync<ContainerInfo>();

        if (containerInfo?.State.Health.Status == "healthy")
        {
            return HealthStatus.Healthy;
        }
        
        return HealthStatus.Unhealthy;
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