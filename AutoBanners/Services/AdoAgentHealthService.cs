using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using AutoBanners.Models;
using AutoBanners.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace AutoBanners.Services;

public class AdoAgentHealthService : HealthServiceBase, IHealthService<AzureDevOpsConfiguration>
{
    protected override string ServiceName => "Azure DevOps";
    
    public AdoAgentHealthService(ILogger<AdoAgentHealthService> logger, HttpClient client)
        : base(logger, client) { }
    
    public Task<HealthStatus> GetHealthAsync(AzureDevOpsConfiguration configuration)
    {
        var authHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($":{configuration.AccessToken}")));
        var url = new Uri($"{configuration.BaseAddress.AbsoluteUri.TrimEnd('/')}/_apis/distributedtask/pools/{configuration.PoolId}/agents?api-version=3.2-preview&agentName={configuration.AgentName}");
        
        HttpRequestMessage requestMessage = new()
        {
            Method = HttpMethod.Get,
            RequestUri = url,
            Headers =
            {
                Authorization = authHeader
            }
        };

        return CheckHealthAsync(requestMessage, async response =>
        {
            var agentHealth = await response.Content.ReadFromJsonAsync<AgentHealthResponse>();

            return agentHealth?.Agents.FirstOrDefault()?.Status == "online"
                ? HealthStatus.Healthy
                : HealthStatus.Unhealthy;
        });
    }
}

class AgentHealthResponse
{
    [JsonPropertyName("value")]
    public Agent[] Agents { get; set; } = [];
}

class Agent
{
    public string Status { get; set; } = string.Empty;
}