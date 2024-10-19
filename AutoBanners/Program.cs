using AutoBanners.Services;

var configurationService = new ConfigurationService();

await configurationService.LoadConfigurationAsync();

if (configurationService.Configuration is null)
{
    throw new Exception("Configuration could not be loaded.");
}

var bannerService = new BannerService(
    new HttpClient(),
    configurationService.Configuration.AzBaseAddress,
    configurationService.Configuration.AzAccessToken);

var client = new HttpClient();
var healthServiceFactory = new HealthServiceFactory();

await new AutoBannersService(bannerService, configurationService, client, healthServiceFactory).RunAsync();
