using AutoBanners.Models;
using AutoBanners.Services;
using AutoBanners.Services.Abstractions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace AutoBanners.Tests.Unit.Services;

public class AutoBannersServiceTests
{
    [Fact]
    public async Task RunAsync_Should_Call_Health_Endpoints()
    {
        // Arrange
        var unhealthyBannerMessage = "Azure DevOps Agent is unhealthy";
        
        var configurationService = Substitute.For<IConfigurationService>();

        configurationService.Configuration.Returns(new Configuration
        {
            HealthEndpoints =
            [
                new HealthEndpoint
                {
                    AzAgent = new AzureDevOpsConfiguration
                    {
                        BaseAddress = new Uri("https://dev.azure.com/health"),
                        UnhealthyBannerMessage = unhealthyBannerMessage,
                    }
                },
                new HealthEndpoint
                {
                    Generic = new GenericConfiguration
                    {
                        HealthEndpoint =
                            new Uri("https://myservice.example.com/health"),
                    }
                },
                new HealthEndpoint
                {
                    NuGet = new NugetConfiguration
                    {
                        HealthEndpoint =
                            new Uri("https://nuget.example.com/health"),
                    }
                },
                new HealthEndpoint
                {
                    Portainer = new PortainerConfiguration
                    {
                        BaseAddress =
                            new Uri("https://portainer.example.com/health"),
                    }
                },
            ]
        });

        var httpClient = new MockHttpMessageHandler().ToHttpClient();
        var healthServiceFactory = Substitute.For<IHealthServiceFactory>();

        var azureDevopsAgentHealthService = Substitute.For<IHealthService<AzureDevOpsConfiguration>>();
        var genericHealthService = Substitute.For<IHealthService<GenericConfiguration>>();
        var nugetHealthService = Substitute.For<IHealthService<NugetConfiguration>>();
        var portainerHealthService = Substitute.For<IHealthService<PortainerConfiguration>>();

        azureDevopsAgentHealthService
            .GetHealthAsync(Arg.Any<AzureDevOpsConfiguration>())
            .Returns(HealthStatus.Unhealthy);
        
        healthServiceFactory
            .Create<AzureDevOpsConfiguration>(httpClient)
            .Returns(azureDevopsAgentHealthService);
        
        healthServiceFactory
            .Create<GenericConfiguration>(httpClient)
            .Returns(genericHealthService);
        
        healthServiceFactory
            .Create<NugetConfiguration>(httpClient)
            .Returns(nugetHealthService);
        
        healthServiceFactory
            .Create<PortainerConfiguration>(httpClient)
            .Returns(portainerHealthService);

        var bannerService = Substitute.For<IBannerService>();
        
        var autoBannersService = new AutoBannersService(
            Substitute.For<ILogger<AutoBannersService>>(),
            bannerService,
            configurationService,
            httpClient,
            healthServiceFactory);

        var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));
        
        // Act
        await autoBannersService.RunAsync(cts.Token);

        // Assert
        await azureDevopsAgentHealthService.Received(1).GetHealthAsync(Arg.Any<AzureDevOpsConfiguration>());
        await genericHealthService.Received(1).GetHealthAsync(Arg.Any<GenericConfiguration>());
        await nugetHealthService.Received(1).GetHealthAsync(Arg.Any<NugetConfiguration>());
        await portainerHealthService.Received(1).GetHealthAsync(Arg.Any<PortainerConfiguration>());
        
        await bannerService.Received(1)
            .CreateBannerAsync(Arg.Is<Banner>(b => b.Message == unhealthyBannerMessage && b.Level == Level.Warning));
    }
}