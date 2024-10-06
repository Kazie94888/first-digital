namespace SmartCoinOS.Integrations.Shared.Exceptions;

/// <summary>
/// Used to indicate an issue with an integration API
/// </summary>
public class ApiException : Exception
{
    public int StatusCode { get; }
    public string? Url { get; }
    public string? Response { get; }

    public ApiException(string url, int statusCode, string response) : base(HumanizeMessage(url, statusCode, response))
    {
        StatusCode = statusCode;
        Url = url;
        Response = response;
    }

    private static string HumanizeMessage(string url, int statusCode, string response)
    {
        var message = $"Request to \"{url}\" failed with status code \"{statusCode}\"";
        if (!string.IsNullOrEmpty(response))
            message += $"\nResponse:\n {response}";
        return message;
    }
}
