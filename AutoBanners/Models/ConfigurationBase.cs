namespace AutoBanners.Models;

public abstract class ConfigurationBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string UnhealthyBannerMessage { get; set; } = string.Empty;
    
    public abstract string GetUnhealthyBannerMessage();
}
