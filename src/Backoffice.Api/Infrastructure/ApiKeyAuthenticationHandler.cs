using System.Security.Claims;
using AspNetCore.Authentication.ApiKey;
using Microsoft.Extensions.Options;
using SmartCoinOS.Backoffice.Api.Base.Options;

namespace SmartCoinOS.Backoffice.Api.Infrastructure;

internal sealed class ApiKeyAuthenticationHandler
{
    private readonly AuthenticationApiKeysConfig _apiKeysConfig;

    public ApiKeyAuthenticationHandler(IOptions<AuthenticationApiKeysConfig> apiKeys)
    {
        _apiKeysConfig = apiKeys.Value;
    }

    public Task ValidateKeyAsync(ApiKeyValidateKeyContext context)
    {
        var serviceName = _apiKeysConfig.Keys.FirstOrDefault(x => x.Value == context.ApiKey).Key;
        if (serviceName is not null)
            context.ValidationSucceeded(new[] { new Claim(ClaimTypes.Spn, serviceName) });
        else
            context.ValidationFailed("Invalid API key");

        return Task.CompletedTask;
    }
}