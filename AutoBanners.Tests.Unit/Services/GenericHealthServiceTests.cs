using System.Net;
using AutoBanners.Models;
using AutoBanners.Services;
using RichardSzalay.MockHttp;
using FluentAssertions;

namespace AutoBanners.Tests.Unit.Services;

public class GenericHealthServiceTests
{
    [Fact]
    public async Task GetHealthAsync_Should_Return_Healthy()
    {
        // Arrange
        var mockHandler = new MockHttpMessageHandler();
        
        string url = "https://example.com/health";

        var mockedRequest = mockHandler
            .When(HttpMethod.Get, url)
            .Respond(HttpStatusCode.OK);
        
        var genericHealthService = new GenericHealthService(mockHandler.ToHttpClient());

        var configuration = new GenericConfiguration()
        {
            HealthEndpoint = new Uri(url)
        };

        // Act
        var response = await genericHealthService.GetHealthAsync(configuration);
        
        // Assert
        response.Should().Be(HealthStatus.Healthy);
        mockHandler.GetMatchCount(mockedRequest).Should().Be(1);
    }
    
    [Fact]
    public async Task GetHealthAsync_Should_Return_Unhealthy_When_Status_Code_Is_Not_Success()
    {
        // Arrange
        var mockHandler = new MockHttpMessageHandler();
        
        string url = "https://example.com/health";

        var mockedRequest = mockHandler
            .When(HttpMethod.Get, url)
            .Respond(HttpStatusCode.ServiceUnavailable);
        
        var genericHealthService = new GenericHealthService(mockHandler.ToHttpClient());

        var configuration = new GenericConfiguration()
        {
            HealthEndpoint = new Uri(url)
        };

        // Act
        var response = await genericHealthService.GetHealthAsync(configuration);
        
        // Assert
        response.Should().Be(HealthStatus.Unhealthy);
        mockHandler.GetMatchCount(mockedRequest).Should().Be(1);
    }
    
    [Fact]
    public async Task GetHealthAsync_Should_Return_Unhealthy_When_Exception_Is_Thrown()
    {
        // Arrange
        var mockHandler = new MockHttpMessageHandler();
        
        string url = "https://example.com/health";

        var mockedRequest = mockHandler
            .When(HttpMethod.Get, url)
            .Throw(new Exception());
        
        var genericHealthService = new GenericHealthService(mockHandler.ToHttpClient());

        var configuration = new GenericConfiguration()
        {
            HealthEndpoint = new Uri(url)
        };

        // Act
        var response = await genericHealthService.GetHealthAsync(configuration);
        
        // Assert
        response.Should().Be(HealthStatus.Unhealthy);
        mockHandler.GetMatchCount(mockedRequest).Should().Be(1);
    }
}