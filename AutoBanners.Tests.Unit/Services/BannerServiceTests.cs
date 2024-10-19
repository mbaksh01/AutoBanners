using System.Net;
using AutoBanners.Models;
using AutoBanners.Services;
using FluentAssertions;
using RichardSzalay.MockHttp;

namespace AutoBanners.Tests.Unit.Services;

public class BannerServiceTests
{
    [Fact]
    public async Task CreateBannerAsync_Should_Create_Banner()
    {
        // Arrange
        var handler = new MockHttpMessageHandler();
        
        var mockedRequest = handler
            .When(HttpMethod.Patch, "https://dev.azure.com/_apis/settings/entries/host/GlobalMessageBanners?api-version=3.2-preview")
            .Respond(HttpStatusCode.NoContent);
        
        var bannerService = new BannerService(handler.ToHttpClient(), new Uri("https://dev.azure.com"), "token");

        var banner = new Banner
        {
            Message = "Hello World",
            Priority = Priority.p1,
            Level = Level.Warning
        };

        // Act
        var title = await bannerService.CreateBannerAsync(banner);

        // Assert
        title.Should().NotBeEmpty();
        title.Should().Contain("GlobalMessageBanners");
        title.Should().Contain("p1");
        handler.GetMatchCount(mockedRequest).Should().Be(1);
    }
    
    [Fact]
    public async Task DeleteBannerAsync_Should_Delete_Banner()
    {
        // Arrange
        var title = Guid.NewGuid().ToString();
        
        var handler = new MockHttpMessageHandler();
        
        var mockedRequest = handler
            .When(HttpMethod.Delete, $"https://dev.azure.com/_apis/settings/entries/host/{title}?api-version=3.2-preview")
            .Respond(HttpStatusCode.NoContent);
        
        var bannerService = new BannerService(handler.ToHttpClient(), new Uri("https://dev.azure.com"), "token");

        // Act
        await bannerService.DeleteBannerAsync(title);

        // Assert
        handler.GetMatchCount(mockedRequest).Should().Be(1);
    }
}