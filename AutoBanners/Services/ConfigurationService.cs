using System.Text.Json;
using AutoBanners.Models;

namespace AutoBanners.Services;

public class ConfigurationService
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    public Configuration? Configuration { get; set; }
    
    public Task LoadConfigurationAsync()
    {
        if (Configuration is not null)
        {
            return Task.CompletedTask;
        }
        
        var file = File.Open("Configuration/Configuration.json", FileMode.Open);

        var configurations = JsonSerializer.Deserialize<Configuration>(file, JsonSerializerOptions);

        if (configurations is not null)
        {
            Configuration = configurations;
        }
        else
        {
            throw new Exception("Could not load the configuration file.");
        }

        if (!string.IsNullOrWhiteSpace(Configuration.AzAccessToken))
        {
            return Task.CompletedTask;
        }
        
        var accessToken = Environment.GetEnvironmentVariable("AZ_ACCESS_TOKEN");

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new Exception("Could not determine the Az Access Token. Set the AZ_ACCESS_TOKEN environment variable or provide its value through the configuration file.");
        }
            
        Configuration.AzAccessToken = accessToken;

        return Task.CompletedTask;
    }
}