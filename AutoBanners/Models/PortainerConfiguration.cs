namespace AutoBanners.Models;

public class PortainerConfiguration : ConfigurationBase
{
    public int EnvironmentId { get; set; }

    public string ContainerName { get; set; } = string.Empty;

    public Uri BaseAddress { get; set; } = new("about:blank");

    public string ApiKey { get; set; } = string.Empty;
    
    public override string GetUnhealthyBannerMessage()
    {
        return string.IsNullOrWhiteSpace(UnhealthyBannerMessage)
            ? $"Portainer container: {ContainerName} is unhealthy."
            : UnhealthyBannerMessage;
    }
}