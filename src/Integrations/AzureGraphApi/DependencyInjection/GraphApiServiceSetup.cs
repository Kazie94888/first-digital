using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using SmartCoinOS.Integrations.AzureGraphApi.Contracts;
using SmartCoinOS.Integrations.AzureGraphApi.Options;

namespace SmartCoinOS.Integrations.AzureGraphApi.DependencyInjection;

public static class GraphApiServiceSetup
{
    public static IServiceCollection AddGraphApiClient(this IServiceCollection services, Action<GraphApiConfig> config)
    {
        services.Configure(config);

        services.AddScoped(provider =>
        {
            var configValue = provider.GetRequiredService<IOptions<GraphApiConfig>>().Value;
            return new GraphServiceClient(new ClientSecretCredential(configValue.TenantId, configValue.ClientId, configValue.ClientSecret));
        });

        services.AddScoped<IGraphApiClient, GraphApiClient>();
        return services;
    }
}