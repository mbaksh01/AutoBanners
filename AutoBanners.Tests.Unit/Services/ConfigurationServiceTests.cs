using System.Text.Json;
using AutoBanners.Models;
using AutoBanners.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AutoBanners.Tests.Unit.Services;

public class ConfigurationServiceTests
{
    [Fact]
    public async Task LoadConfiguration_Should_Correctly_Load_Configuration()
    {
        // Arrange
        var configuration = new Configuration
        {
            AzBaseAddress = new Uri("https://dev.azure.com/MyOrg"),
            HealthEndpoints =
            [
                new HealthEndpoint
                {
                    AzAgent = new AzureDevOpsConfiguration
                    {
                        BaseAddress = new Uri("https://dev.azure.com/health"),
                    }
                },
                new HealthEndpoint
                {
                    Generic = new GenericConfiguration
                    {
                        HealthEndpoint = new Uri("https://myservice.example.com/health"),
                    }
                },
                new HealthEndpoint
                {
                    NuGet = new NugetConfiguration
                    {
                        HealthEndpoint = new Uri("https://nuget.example.com/health"),
                    }
                },
                new HealthEndpoint
                {
                    Portainer = new PortainerConfiguration
                    {
                        BaseAddress = new Uri("https://portainer.example.com/health"),
                    }
                },
            ]
        };
        
        if (File.Exists("Configuration/Configuration.json"))
        {
            File.Delete("Configuration/Configuration.json");
        }
        
        Directory.CreateDirectory("Configuration");
        var configFile = File.Create("Configuration/Configuration.json");
        var writer = new StreamWriter(configFile);

        await writer.WriteAsync(JsonSerializer.Serialize(configuration));
        
        await writer.DisposeAsync();
        await configFile.DisposeAsync();
        
        Environment.SetEnvironmentVariable("AZ_ACCESS_TOKEN", "my-token");

        configuration.AzAccessToken = "my-token";
        
        var configurationService = new ConfigurationService(Substitute.For<ILogger<ConfigurationService>>());

        // Act
        await configurationService.LoadConfigurationAsync();

        // Assert
        configurationService.Configuration.Should().BeEquivalentTo(configuration);
    }
}