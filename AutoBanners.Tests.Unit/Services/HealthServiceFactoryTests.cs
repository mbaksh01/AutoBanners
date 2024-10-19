using AutoBanners.Models;
using AutoBanners.Services;
using FluentAssertions;
using RichardSzalay.MockHttp;

namespace AutoBanners.Tests.Unit.Services;

public class HealthServiceFactoryTests
{
    [Fact]
    public void Create_Should_Create_AzureDevOpsHealthService()
    {
        // Arrange
        var healthServiceFactory = new HealthServiceFactory();

        // Act
        var healthService = healthServiceFactory.Create<AzureDevOpsConfiguration>(
            new MockHttpMessageHandler().ToHttpClient());

        // Assert
        healthService.Should().BeOfType<AzureDevOpsAgentHealthService>();
    }
    
    [Fact]
    public void Create_Should_Create_GenericHealthService()
    {
        // Arrange
        var healthServiceFactory = new HealthServiceFactory();

        // Act
        var healthService = healthServiceFactory.Create<GenericConfiguration>(
            new MockHttpMessageHandler().ToHttpClient());

        // Assert
        healthService.Should().BeOfType<GenericHealthService>();
    }
    
    [Fact]
    public void Create_Should_Create_NugetHealthService()
    {
        // Arrange
        var healthServiceFactory = new HealthServiceFactory();

        // Act
        var healthService = healthServiceFactory.Create<NugetConfiguration>(
            new MockHttpMessageHandler().ToHttpClient());

        // Assert
        healthService.Should().BeOfType<NugetHealthService>();
    }
    
    [Fact]
    public void Create_Should_Create_PortainerHealthService()
    {
        // Arrange
        var healthServiceFactory = new HealthServiceFactory();

        // Act
        var healthService = healthServiceFactory.Create<PortainerConfiguration>(
            new MockHttpMessageHandler().ToHttpClient());

        // Assert
        healthService.Should().BeOfType<PortainerHealthService>();
    }
    
    [Fact]
    public void Create_Should_Throw_NotSupportedException()
    {
        // Arrange
        var healthServiceFactory = new HealthServiceFactory();

        // Act
        Action act = () => healthServiceFactory.Create<ConfigurationBase>(
            new MockHttpMessageHandler().ToHttpClient());

        // Assert
        act.Should().Throw<NotSupportedException>();
    }
}