using AutoBanners.Models;

namespace AutoBanners.Services.Abstractions;

public interface IHealthService<in TConfiguration>
    where TConfiguration : class
{
    Task<HealthStatus> GetHealthAsync(TConfiguration configuration);
}