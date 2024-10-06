using System.Net.Http.Headers;
using IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SmartCoinOS.Integrations.FdtPartnerApi.Contracts;
using SmartCoinOS.Integrations.FdtPartnerApi.Services;


namespace SmartCoinOS.Integrations.FdtPartnerApi.DependencyInjection;
public static class FdtPartnerApiServiceSetup
{
    public static IServiceCollection AddFdtPartnerApi(this IServiceCollection services, Action<FdtPartnerSettings> config)
    {
        services.Configure(config);

        services.AddHttpClient<IFdtPartnerApiClient, FdtPartnerApiClient>((serviceProvider, httpClient) =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<FdtPartnerSettings>>().Value;

            httpClient.BaseAddress = new Uri(settings.ApiUrl);
            httpClient.Timeout = TimeSpan.FromSeconds(settings.ApiTimeout);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }).AddClientAccessTokenHandler(nameof(FdtPartnerApiClient));

        services.AddAccessTokenManagement((serviceProvider, options) =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<FdtPartnerSettings>>().Value;

            options.Client.Clients.Add(nameof(FdtPartnerApiClient), new ClientCredentialsTokenRequest
            {
                Address = "https://login.microsoftonline.com/common/oauth2/v2.0/token",
                ClientId = settings.ApiClientId,
                ClientSecret = settings.ApiClientSecret,
                Scope = settings.ApiScope,
                GrantType = "client_credentials"
            });
        });


        return services;
    }
}
