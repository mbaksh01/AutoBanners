namespace AutoBanners.Models;

public class NugetConfiguration : ConfigurationBase
{
    public Uri HealthEndpoint { get; set; } = new("about:blank");
    
    public override string GetUnhealthyBannerMessage()
    {
        return string.IsNullOrWhiteSpace(UnhealthyBannerMessage)
            ? "NuGet server is unhealthy."
            : UnhealthyBannerMessage;
    }
}