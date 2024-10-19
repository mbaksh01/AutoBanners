using System.Net;
using AutoBanners.Models;
using AutoBanners.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace AutoBanners.Tests.Unit.Services;

public class NugetHealthServiceTests
{
    [Fact]
    public async Task GetHealthAsync_Should_Return_Healthy()
    {
        // Arrange
        string url = "https://nuget.org/health";
        
        var mockHandler = new MockHttpMessageHandler();

        var mockedRequest = mockHandler
            .When(HttpMethod.Get, url)
            .Respond(_ => new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"Status\":\"Healthy\"}")
            });
        
        var nugetConfiguration = new NugetConfiguration
        {
            HealthEndpoint = new Uri(url)
        };

        var healthService = new NugetHealthService(
            Substitute.For<ILogger<NugetHealthService>>(),
            mockHandler.ToHttpClient());

        // Act
        var response = await healthService.GetHealthAsync(nugetConfiguration);

        // Assert
        response.Should().Be(HealthStatus.Healthy);
        mockHandler.GetMatchCount(mockedRequest).Should().Be(1);
    }
    
    [Fact]
    public async Task GetHealthAsync_Should_Return_Unhealthy_When_Response_Body_Is_Unhealthy()
    {
        // Arrange
        string url = "https://nuget.org/health";
        
        var mockHandler = new MockHttpMessageHandler();

        var mockedRequest = mockHandler
            .When(HttpMethod.Get, url)
            .Respond(_ => new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"Status\":\"Unhealthy\"}")
            });
        
        var nugetConfiguration = new NugetConfiguration
        {
            HealthEndpoint = new Uri(url)
        };

        var healthService = new NugetHealthService(
            Substitute.For<ILogger<NugetHealthService>>(),
            mockHandler.ToHttpClient());

        // Act
        var response = await healthService.GetHealthAsync(nugetConfiguration);

        // Assert
        response.Should().Be(HealthStatus.Unhealthy);
        mockHandler.GetMatchCount(mockedRequest).Should().Be(1);
    }
    
    [Fact]
    public async Task GetHealthAsync_Should_Return_Unhealthy_When_Status_Code_Is_Not_Success()
    {
        // Arrange
        string url = "https://nuget.org/health";
        
        var mockHandler = new MockHttpMessageHandler();

        var mockedRequest = mockHandler
            .When(HttpMethod.Get, url)
            .Respond(HttpStatusCode.ServiceUnavailable);
        
        var nugetConfiguration = new NugetConfiguration
        {
            HealthEndpoint = new Uri(url)
        };

        var healthService = new NugetHealthService(
            Substitute.For<ILogger<NugetHealthService>>(),
            mockHandler.ToHttpClient());

        // Act
        var response = await healthService.GetHealthAsync(nugetConfiguration);

        // Assert
        response.Should().Be(HealthStatus.Unhealthy);
        mockHandler.GetMatchCount(mockedRequest).Should().Be(1);
    }
}