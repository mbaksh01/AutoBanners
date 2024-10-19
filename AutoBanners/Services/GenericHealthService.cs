using AutoBanners.Models;
using AutoBanners.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace AutoBanners.Services;

public class GenericHealthService : HealthServiceBase, IHealthService<GenericConfiguration>
{
    protected override string ServiceName => "Generic Health Service";

    public GenericHealthService(ILogger<GenericHealthService> logger, HttpClient client)
        : base(logger, client) { }
    
    public Task<HealthStatus> GetHealthAsync(GenericConfiguration configuration)
    {
        return CheckHealthAsync(configuration.HealthEndpoint, response =>
        {
            var status = response.IsSuccessStatusCode ? HealthStatus.Healthy : HealthStatus.Unhealthy;

            return Task.FromResult(status);
        });
    }
}