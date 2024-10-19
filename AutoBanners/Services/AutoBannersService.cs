using AutoBanners.Models;

namespace AutoBanners.Services;

public class AutoBannersService
{
    private readonly IBannerService _bannerService;
    private readonly IHealthServiceFactory _healthServiceFactory;
    private readonly ConfigurationService _configurationService;
    private readonly HttpClient _client;
    private readonly Dictionary<Guid, string> _activeBanners = [];

    public AutoBannersService(
        IBannerService bannerService,
        ConfigurationService configurationService,
        HttpClient client,
        IHealthServiceFactory healthServiceFactory)
    {
        _bannerService = bannerService;
        _configurationService = configurationService;
        _client = client;
        _healthServiceFactory = healthServiceFactory;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        await _configurationService.LoadConfigurationAsync();

        if (_configurationService.Configuration is null)
        {
            throw new Exception("Configuration file has not been loaded.");
        }
        
        while (!cancellationToken.IsCancellationRequested)
        {
            foreach (var healthEndpoint in _configurationService.Configuration.HealthEndpoints)
            {
                if (healthEndpoint.NuGet is not null)
                {
                    await CheckHealthAndCreateBannerAsync(
                        _healthServiceFactory.Create<NugetConfiguration>(_client),
                        healthEndpoint.NuGet);
                }
                
                if (healthEndpoint.Portainer is not null)
                {
                    await CheckHealthAndCreateBannerAsync(
                        _healthServiceFactory.Create<PortainerConfiguration>(_client),
                        healthEndpoint.Portainer);
                }
                
                if (healthEndpoint.AzAgent is not null)
                {
                    await CheckHealthAndCreateBannerAsync(
                        _healthServiceFactory.Create<AzureDevOpsConfiguration>(_client),
                        healthEndpoint.AzAgent);
                }

                if (healthEndpoint.Generic is not null)
                {
                    await CheckHealthAndCreateBannerAsync(
                        _healthServiceFactory.Create<GenericConfiguration>(_client),
                        healthEndpoint.Generic);
                }
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            }
            catch (TaskCanceledException)
            {
            }
        }
    }
    
    private async Task CheckHealthAndCreateBannerAsync<TConfiguration>(
        IHealthService<TConfiguration> healthService,
        TConfiguration configuration)
        where TConfiguration : ConfigurationBase
    {
        var health = await healthService.GetHealthAsync(configuration);

        if (health == HealthStatus.Unhealthy && !_activeBanners.ContainsKey(configuration.Id))
        {
            var title = await _bannerService.CreateBannerAsync(new Banner
            {
                Message = configuration.GetUnhealthyBannerMessage(),
                Level = Level.Warning,
                Priority = Priority.p0
            });
            
            _activeBanners.Add(configuration.Id, title);
        }
        else if (health == HealthStatus.Healthy)
        {
            if (_activeBanners.TryGetValue(configuration.Id, out string? title))
            {
                await _bannerService.DeleteBannerAsync(title);
            }
        }
    }
}