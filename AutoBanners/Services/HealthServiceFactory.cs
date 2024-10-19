using AutoBanners.Models;
using AutoBanners.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AutoBanners.Services;

public class HealthServiceFactory : IHealthServiceFactory
{
    private readonly IServiceProvider _serviceProvider;
    
    public HealthServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public IHealthService<TConfiguration> Create<TConfiguration>(HttpClient client)
        where TConfiguration : class
    {
        return typeof(TConfiguration).Name switch
        {
            nameof(AzureDevOpsConfiguration) => (IHealthService<TConfiguration>)new AdoAgentHealthService(
                _serviceProvider.GetRequiredService<ILogger<AdoAgentHealthService>>(), client),
            nameof(GenericConfiguration) => (IHealthService<TConfiguration>)new GenericHealthService(
                _serviceProvider.GetRequiredService<ILogger<GenericHealthService>>(), client),
            nameof(NugetConfiguration) => (IHealthService<TConfiguration>)new NugetHealthService(
                _serviceProvider.GetRequiredService<ILogger<NugetHealthService>>(), client),
            nameof(PortainerConfiguration) => (IHealthService<TConfiguration>)new PortainerHealthService(
                _serviceProvider.GetRequiredService<ILogger<PortainerHealthService>>(), client),
            _ => throw new NotSupportedException()
        };
    }
}