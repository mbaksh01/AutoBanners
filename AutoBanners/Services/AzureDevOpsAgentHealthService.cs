using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using AutoBanners.Models;

namespace AutoBanners.Services;

public class AzureDevOpsAgentHealthService : IHealthService<AzureDevOpsConfiguration>
{
    private readonly HttpClient _client;
    
    public AzureDevOpsAgentHealthService(HttpClient client)
    {
        _client = client;
    }
    
    public async Task<HealthStatus> GetHealthAsync(AzureDevOpsConfiguration configuration)
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

        var response = await _client.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            return HealthStatus.Unhealthy;
        }

        var agentHealth = await response.Content.ReadFromJsonAsync<AgentHealthResponse>();

        return agentHealth?.Agents.FirstOrDefault()?.Status == "online"
            ? HealthStatus.Healthy
            : HealthStatus.Unhealthy;
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