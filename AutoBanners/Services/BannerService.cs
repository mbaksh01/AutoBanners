using System.Net;
using System.Net.Http.Headers;
using System.Text;
using AutoBanners.Models;
using AutoBanners.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace AutoBanners.Services;

public class BannerService : IBannerService
{
    private readonly ILogger<BannerService> _logger;
    private readonly IConfigurationService _configurationService;
    private readonly HttpClient _client;
    private Uri? _baseAddress;
    private string _accessToken = string.Empty;
    
    public BannerService(
        ILogger<BannerService> logger,
        HttpClient client,
        IConfigurationService configurationService)
    {
        _logger = logger;
        _client = client;
        _configurationService = configurationService;
    }
    
    public async Task<string> CreateBannerAsync(Banner banner)
    {
        await EnsureConfigurationIsLoadedAsync();
        
        var title = $"GlobalMessageBanners/{banner.Priority}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        var json = $$"""
                    {
                        "{{title}}": {
                            "level": "{{banner.Level}}",
                            "message": "{{banner.Message}}"
                        }
                    }
                    """;
        
        var content = new StringContent(json, new MediaTypeHeaderValue("application/json"));
        var authHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($":{_accessToken}")));
        var url = new Uri($"{_baseAddress!.AbsoluteUri.TrimEnd('/')}/_apis/settings/entries/host/GlobalMessageBanners?api-version=3.2-preview");
        
        HttpRequestMessage requestMessage = new()
        {
            Method = HttpMethod.Patch,
            RequestUri = url,
            Content = content,
            Headers =
            {
                Authorization = authHeader
            }
        };

        var response = await _client.SendAsync(requestMessage);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(
                "Created banner with message '{Message}'.",
                banner.Message);
        }
        else if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
        {
            _logger.LogError("Failed to create banner. The request was unauthorized. The Az token must have full access to the organization and must be from a user who is part of the Project Collection Administrators.");
        }
        else
        {
            _logger.LogError(
                "Failed to create banner. Azure DevOps response: {Response}",
                await response.Content.ReadAsStreamAsync());
        }

        return title;
    }
    
    public async Task DeleteBannerAsync(string title)
    {
        await EnsureConfigurationIsLoadedAsync();
        
        var authHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($":{_accessToken}")));
        var url = new Uri($"{_baseAddress!.AbsoluteUri.TrimEnd('/')}/_apis/settings/entries/host/{title}?api-version=3.2-preview");
        
        HttpRequestMessage requestMessage = new()
        {
            Method = HttpMethod.Delete,
            RequestUri = url,
            Headers =
            {
                Authorization = authHeader
            }
        };

        var response = await _client.SendAsync(requestMessage);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Deleted banner.");
        }
        else
        {
            _logger.LogInformation(
                "Failed to delete banner. Azure DevOps response: {Response}",
                await response.Content.ReadAsStringAsync());
        }
    }
    
    private async Task EnsureConfigurationIsLoadedAsync()
    {
        if (_baseAddress is not null && !string.IsNullOrWhiteSpace(_accessToken))
        {
            return;
        }

        await _configurationService.LoadConfigurationAsync();

        _baseAddress = _configurationService.Configuration?.AzBaseAddress;
        _accessToken = _configurationService.Configuration?.AzAccessToken ?? string.Empty;
        
        if (_baseAddress is null || string.IsNullOrWhiteSpace(_accessToken))
        {
            throw new InvalidOperationException("Az base address and Az token could not be determined.");
        }
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}