namespace AutoBanners.Models;

public class GenericConfiguration : ConfigurationBase
{
    public Uri HealthEndpoint { get; set; } = new("about:blank");
    
    public override string GetUnhealthyBannerMessage()
    {
        return string.IsNullOrWhiteSpace(UnhealthyBannerMessage)
            ? $"[Service]({HealthEndpoint}) is unhealthy."
            : UnhealthyBannerMessage;
    }
}