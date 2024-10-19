namespace AutoBanners.Models;

public class AzureDevOpsConfiguration : ConfigurationBase
{
    public Uri BaseAddress { get; set; } = new("about:blank");

    public string AccessToken { get; set; } = string.Empty;

    public int PoolId { get; set; }
    
    public string AgentName { get; set; } = string.Empty;
    
    public override string GetUnhealthyBannerMessage()
    {
        return string.IsNullOrWhiteSpace(UnhealthyBannerMessage)
            ? $"{AgentName} is offline."
            : UnhealthyBannerMessage;
    }
}