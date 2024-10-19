using AutoBanners.Models;

namespace AutoBanners.Services;

public class HealthServiceFactory : IHealthServiceFactory
{
    public IHealthService<TConfiguration> Create<TConfiguration>(HttpClient client)
        where TConfiguration : class
    {
        return typeof(TConfiguration).Name switch
        {
            nameof(AzureDevOpsConfiguration) => (IHealthService<TConfiguration>)new AzureDevOpsAgentHealthService(client),
            nameof(GenericConfiguration) => (IHealthService<TConfiguration>)new GenericHealthService(client),
            nameof(NugetConfiguration) => (IHealthService<TConfiguration>)new NugetHealthService(client),
            nameof(PortainerConfiguration) => (IHealthService<TConfiguration>)new PortainerHealthService(client),
            _ => throw new NotSupportedException()
        };
    }
}