using System.Net;
using AutoBanners.Models;
using AutoBanners.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace AutoBanners.Tests.Unit.Services;

public class PortainerHealthServiceTests
{
    [Fact]
    public async Task GetHealthAsync_Should_Return_Healthy()
    {
        // Arrange
        const string url = "https://portainer.example.com:9000";
        const int environmentId = 1;
        const string containerName = "my-container";
        var mockHandler = new MockHttpMessageHandler();
        
        mockHandler
            .When(HttpMethod.Get, $"{url}/api/endpoints/{environmentId}/docker/containers/{containerName}/json")
            .Respond("application/json", "{\"State\": {\"Health\": {\"Status\": \"healthy\"}}}");

        var healthService = new PortainerHealthService(
            Substitute.For<ILogger<PortainerHealthService>>(),
            mockHandler.ToHttpClient());

        var config = new PortainerConfiguration
        {
            BaseAddress = new Uri(url),
            EnvironmentId = environmentId,
            ContainerName = containerName,
        };

        // Act
        var response = await healthService.GetHealthAsync(config);

        // Assert
        response.Should().Be(HealthStatus.Healthy);
    }
    
    [Fact]
    public async Task GetHealthAsync_Should_Return_Unhealthy_When_Status_Code_Is_Not_Success()
    {
        // Arrange
        const string url = "https://portainer.example.com:9000";
        const int environmentId = 1;
        const string containerName = "my-container";
        var mockHandler = new MockHttpMessageHandler();
        
        mockHandler
            .When(HttpMethod.Get, $"{url}/api/endpoints/{environmentId}/docker/containers/{containerName}/json")
            .Respond(HttpStatusCode.ServiceUnavailable);

        var healthService = new PortainerHealthService(
            Substitute.For<ILogger<PortainerHealthService>>(),
            mockHandler.ToHttpClient());

        var config = new PortainerConfiguration
        {
            BaseAddress = new Uri(url),
            EnvironmentId = environmentId,
            ContainerName = containerName,
        };

        // Act
        var response = await healthService.GetHealthAsync(config);

        // Assert
        response.Should().Be(HealthStatus.Unhealthy);
    }
    
    [Fact]
    public async Task GetHealthAsync_Should_Return_Unhealthy_When_Response_Body_Is_Unhealthy()
    {
        // Arrange
        const string url = "https://portainer.example.com:9000";
        const int environmentId = 1;
        const string containerName = "my-container";
        var mockHandler = new MockHttpMessageHandler();
        
        mockHandler
            .When(HttpMethod.Get, $"{url}/api/endpoints/{environmentId}/docker/containers/{containerName}/json")
            .Respond("application/json", "{\"State\": {\"Health\": {\"Status\": \"unhealthy\"}}}");

        var healthService = new PortainerHealthService(
            Substitute.For<ILogger<PortainerHealthService>>(),
            mockHandler.ToHttpClient());

        var config = new PortainerConfiguration
        {
            BaseAddress = new Uri(url),
            EnvironmentId = environmentId,
            ContainerName = containerName,
        };

        // Act
        var response = await healthService.GetHealthAsync(config);

        // Assert
        response.Should().Be(HealthStatus.Unhealthy);
    }
}