using System.Net.Http.Headers;
using System.Text;
using AutoBanners.Models;

namespace AutoBanners.Services;

public class BannerService : IBannerService, IDisposable
{
    private readonly HttpClient _client;
    private readonly Uri _baseAddress;
    private readonly string _accessToken;
    
    public BannerService(HttpClient client, Uri baseAddress, string accessToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);
        
        _client = client;
        _baseAddress = baseAddress;
        _accessToken = accessToken;
    }
    
    public async Task<string> CreateBannerAsync(Banner banner)
    {
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
        var url = new Uri($"{_baseAddress.AbsoluteUri.TrimEnd('/')}/_apis/settings/entries/host/GlobalMessageBanners?api-version=3.2-preview");
        
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

        response.EnsureSuccessStatusCode();

        return title;
    }
    
    public async Task DeleteBannerAsync(string title)
    {
        var authHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($":{_accessToken}")));
        var url = new Uri($"{_baseAddress.AbsoluteUri.TrimEnd('/')}/_apis/settings/entries/host/{title}?api-version=3.2-preview");
        
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

        response.EnsureSuccessStatusCode();
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}