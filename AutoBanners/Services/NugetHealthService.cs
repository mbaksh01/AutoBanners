using System.Net.Http.Json;
using System.Text.Json;
using AutoBanners.Models;
using AutoBanners.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace AutoBanners.Services;

public class NugetHealthService : HealthServiceBase, IHealthService<NugetConfiguration>
{
    protected override string ServiceName => "Nuget";
    
    public NugetHealthService(ILogger<NugetHealthService> logger, HttpClient client)
        : base(logger, client) { }
    
    public Task<HealthStatus> GetHealthAsync(NugetConfiguration configuration)
    {
        return CheckHealthAsync(configuration.HealthEndpoint, async response =>
        {
            var healthResponse = await response.Content.ReadFromJsonAsync<NugetHealthResponse>();

            return healthResponse?.Status == "Healthy"
                ? HealthStatus.Healthy
                : HealthStatus.Unhealthy;
        });
    }
}

class NugetHealthResponse
{
    public string Status { get; set; } = string.Empty;
}