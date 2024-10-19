using AutoBanners.Models;
using AutoBanners.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace AutoBanners.Tests.Unit.Services;

public class HealthServiceFactoryTests
{
    [Fact]
    public void Create_Should_Create_AzureDevOpsHealthService()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();

        serviceProvider
            .GetService(typeof(ILogger<AdoAgentHealthService>))
            .Returns(Substitute.For<ILogger<AdoAgentHealthService>>());
        
        var healthServiceFactory = new HealthServiceFactory(serviceProvider);

        // Act
        var healthService = healthServiceFactory.Create<AzureDevOpsConfiguration>(
            new MockHttpMessageHandler().ToHttpClient());

        // Assert
        healthService.Should().BeOfType<AdoAgentHealthService>();
    }
    
    [Fact]
    public void Create_Should_Create_GenericHealthService()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();

        serviceProvider
            .GetService(typeof(ILogger<GenericHealthService>))
            .Returns(Substitute.For<ILogger<GenericHealthService>>());
        
        var healthServiceFactory = new HealthServiceFactory(serviceProvider);

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
        var serviceProvider = Substitute.For<IServiceProvider>();

        serviceProvider
            .GetService(typeof(ILogger<NugetHealthService>))
            .Returns(Substitute.For<ILogger<NugetHealthService>>());
        
        var healthServiceFactory = new HealthServiceFactory(serviceProvider);

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
        var serviceProvider = Substitute.For<IServiceProvider>();

        serviceProvider
            .GetService(typeof(ILogger<PortainerHealthService>))
            .Returns(Substitute.For<ILogger<PortainerHealthService>>());
        
        var healthServiceFactory = new HealthServiceFactory(serviceProvider);

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
        var healthServiceFactory = new HealthServiceFactory(Substitute.For<IServiceProvider>());

        // Act
        Action act = () => healthServiceFactory.Create<ConfigurationBase>(
            new MockHttpMessageHandler().ToHttpClient());

        // Assert
        act.Should().Throw<NotSupportedException>();
    }
}