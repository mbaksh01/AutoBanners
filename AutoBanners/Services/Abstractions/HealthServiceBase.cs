using System.Net;
using System.Text.Json;
using AutoBanners.Models;
using Microsoft.Extensions.Logging;

namespace AutoBanners.Services.Abstractions;

public abstract class HealthServiceBase
{
    private readonly ILogger<HealthServiceBase> _logger;
    private readonly HttpClient _client;
    
    protected abstract string ServiceName { get; }
    
    protected HealthServiceBase(
        ILogger<HealthServiceBase> logger,
        HttpClient client)
    {
        _logger = logger;
        _client = client;
    }
    
    protected Task<HealthStatus> CheckHealthAsync(
        Uri healthEndpoint,
        Func<HttpResponseMessage, Task<HealthStatus>> predicate)
    {
        HttpRequestMessage request = new()
        {
            Method = HttpMethod.Get,
            RequestUri = healthEndpoint
        };

        return CheckHealthAsync(request, predicate);
    }
    
    protected async Task<HealthStatus> CheckHealthAsync(
        HttpRequestMessage request,
        Func<HttpResponseMessage, Task<HealthStatus>> predicate)
    {
        try
        {
            var response = await _client.SendAsync(request);

            if (response.StatusCode is HttpStatusCode.Unauthorized
                or HttpStatusCode.Forbidden)
            {
                _logger.LogWarning(
                    "The request to {HealthEndpoint} was unauthorized. The authentication method may no longer be valid.",
                    request.RequestUri);

                return HealthStatus.Unknown;
            }

            if (response.StatusCode is HttpStatusCode.InternalServerError
                or HttpStatusCode.ServiceUnavailable)
            {
                return HealthStatus.Unhealthy;
            }

            try
            {
                return await predicate.Invoke(response);
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(
                    ex,
                    "The response from {ServiceName} could not be deserialized.",
                    ServiceName);

                return HealthStatus.Unknown;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(
                ex,
                "Could not connect to {ServiceName}.",
                ServiceName);

            return HealthStatus.Unknown;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "The request to {ServiceName} timed out.", ServiceName);

            return HealthStatus.Unhealthy;
        }
    }
}