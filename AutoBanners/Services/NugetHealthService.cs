using System.Net.Http.Json;
using AutoBanners.Models;

namespace AutoBanners.Services;

public class NugetHealthService : IHealthService<NugetConfiguration>
{
    private readonly HttpClient _client;
    
    public NugetHealthService(HttpClient client)
    {
        _client = client;
    }
    
    public async Task<HealthStatus> GetHealthAsync(NugetConfiguration configuration)
    {
        var response = await _client.GetAsync(configuration.HealthEndpoint);

        if (!response.IsSuccessStatusCode)
        {
            return HealthStatus.Unhealthy;
        }
        
        var healthResponse = await response.Content.ReadFromJsonAsync<NugetHealthResponse>();

        return healthResponse?.Status == "Healthy"
            ? HealthStatus.Healthy
            : HealthStatus.Unhealthy;
    }
}

class NugetHealthResponse
{
    public string Status { get; set; } = string.Empty;
}