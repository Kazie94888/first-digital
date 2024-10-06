using System.Text;
using System.Text.Json;
using SmartCoinOS.Integrations.Shared.Exceptions;

namespace SmartCoinOS.Integrations.Shared;
public abstract class HttpClientBase
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    protected HttpClientBase(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    protected async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(url, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var responseCode = (int)response.StatusCode;

        if (!response.IsSuccessStatusCode)
            throw new ApiException(url, responseCode, responseContent);

        var parsedResponse = JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);

        if (parsedResponse is null)
            throw new ApiException(url, responseCode, $"Failed to deserialize the response. Manual check is required. \nResponse:\n {responseContent}");

        return parsedResponse;
    }

    protected async Task PostAsync<TRequest>(string url, TRequest request, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(request, _jsonOptions);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };

        var response = await _httpClient.SendAsync(requestMessage, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var responseCode = (int)response.StatusCode;

        if (!response.IsSuccessStatusCode)
            throw new ApiException(url, responseCode, responseContent);
    }

    protected async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest request, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(request, _jsonOptions);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };

        var response = await _httpClient.SendAsync(requestMessage, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var responseCode = (int)response.StatusCode;

        if (!response.IsSuccessStatusCode)
            throw new ApiException(url, responseCode, responseContent);

        var parsedResponse = JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);

        if (parsedResponse is null)
            throw new ApiException(url, responseCode, $"Failed to deserialize the response. Manual check is required. \nResponse:\n {responseContent}");

        return parsedResponse;
    }
}