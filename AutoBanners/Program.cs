using AutoBanners.Services;
using AutoBanners.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

var services = new ServiceCollection();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

services.AddSingleton<IConfigurationService, ConfigurationService>();
services.AddSingleton<IBannerService, BannerService>();
services.AddSingleton<IHealthServiceFactory, HealthServiceFactory>();
services.AddSingleton<HttpClient>();
services.AddSingleton<AutoBannersService>();
services.AddLogging(builder =>
{
    builder.ClearProviders();
    builder.AddSerilog();
});

var serviceProvider = services.BuildServiceProvider();

await serviceProvider.GetRequiredService<AutoBannersService>().RunAsync();