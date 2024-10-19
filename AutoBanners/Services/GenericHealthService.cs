using AutoBanners.Models;

namespace AutoBanners.Services;

public class GenericHealthService : IHealthService<GenericConfiguration>
{
    private readonly HttpClient _client;
    
    public GenericHealthService(HttpClient client)
    {
        _client = client;
    }
    
    public async Task<HealthStatus> GetHealthAsync(GenericConfiguration configuration)
    {
        try
        {
            var response = await _client.GetAsync(configuration.HealthEndpoint);

            return response.IsSuccessStatusCode ? HealthStatus.Healthy : HealthStatus.Unhealthy;
        }
        catch
        {
            return HealthStatus.Unhealthy;
        }
    }
}