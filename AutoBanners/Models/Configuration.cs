namespace AutoBanners.Models;

public class Configuration
{
    public Uri AzBaseAddress { get; set; } = new("about:blank");

    public string AzAccessToken { get; set; } = string.Empty;

    public List<HealthEndpoint> HealthEndpoints { get; set; } = [];
}