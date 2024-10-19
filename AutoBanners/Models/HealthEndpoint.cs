namespace AutoBanners.Models;

public class HealthEndpoint
{
    public NugetConfiguration? NuGet { get; set; }

    public PortainerConfiguration? Portainer { get; set; }

    public AzureDevOpsConfiguration? AzAgent { get; set; }
    
    public GenericConfiguration? Generic { get; set; }
}