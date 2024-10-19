using System.Net;
using AutoBanners.Models;
using AutoBanners.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace AutoBanners.Tests.Unit.Services;

public class AdoAgentHealthServiceTests
{
    [Fact]
    public async Task GetHealthAsync_Should_Return_Healthy()
    {
        // Arrange
        const string url = "https://dev.azure.com";
        const string agentName = "my-agent";
        int poolId = Random.Shared.Next();
        var mockHandler = new MockHttpMessageHandler();
        
        var mockedRequest = mockHandler
            .When(HttpMethod.Get, $"{url}/_apis/distributedtask/pools/{poolId}/agents?api-version=3.2-preview&agentName={agentName}")
            .Respond("application/json", "{\"value\": [{\"Status\": \"online\"}]}");

        var healthService = new AdoAgentHealthService(Substitute.For<ILogger<AdoAgentHealthService>>(), mockHandler.ToHttpClient());

        var config = new AzureDevOpsConfiguration
        {
            BaseAddress = new Uri(url),
            PoolId = poolId,
            AgentName = agentName
        };

        // Act
        var response = await healthService.GetHealthAsync(config);

        // Assert
        response.Should().Be(HealthStatus.Healthy);
        mockHandler.GetMatchCount(mockedRequest).Should().Be(1);
    }
    
    [Fact]
    public async Task GetHealthAsync_Should_Return_Unhealthy_When_Agent_Is_Offline()
    {
        // Arrange
        const string url = "https://dev.azure.com";
        const string agentName = "my-agent";
        int poolId = Random.Shared.Next();
        var mockHandler = new MockHttpMessageHandler();
        
        var mockedRequest = mockHandler
            .When(HttpMethod.Get, $"{url}/_apis/distributedtask/pools/{poolId}/agents?api-version=3.2-preview&agentName={agentName}")
            .Respond("application/json", "{\"value\": [{\"Status\": \"offline\"}]}");

        var healthService = new AdoAgentHealthService(Substitute.For<ILogger<AdoAgentHealthService>>(), mockHandler.ToHttpClient());

        var config = new AzureDevOpsConfiguration
        {
            BaseAddress = new Uri(url),
            PoolId = poolId,
            AgentName = agentName
        };

        // Act
        var response = await healthService.GetHealthAsync(config);

        // Assert
        response.Should().Be(HealthStatus.Unhealthy);
        mockHandler.GetMatchCount(mockedRequest).Should().Be(1);
    }
    
    [Fact]
    public async Task GetHealthAsync_Should_Return_Unhealthy_When_Status_Code_Is_Not_Success()
    {
        // Arrange
        const string url = "https://dev.azure.com";
        const string agentName = "my-agent";
        int poolId = Random.Shared.Next();
        var mockHandler = new MockHttpMessageHandler();
        
        var mockedRequest = mockHandler
            .When(HttpMethod.Get, $"{url}/_apis/distributedtask/pools/{poolId}/agents?api-version=3.2-preview&agentName={agentName}")
            .Respond(HttpStatusCode.ServiceUnavailable);

        var healthService = new AdoAgentHealthService(Substitute.For<ILogger<AdoAgentHealthService>>(), mockHandler.ToHttpClient());

        var config = new AzureDevOpsConfiguration
        {
            BaseAddress = new Uri(url),
            PoolId = poolId,
            AgentName = agentName
        };

        // Act
        var response = await healthService.GetHealthAsync(config);

        // Assert
        response.Should().Be(HealthStatus.Unhealthy);
        mockHandler.GetMatchCount(mockedRequest).Should().Be(1);
    }
}