using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using SmartCoinOS.Integrations.GnosisSafeApi.Contracts;
using SmartCoinOS.Integrations.GnosisSafeApi.Services;

namespace SmartCoinOS.Integrations.GnosisSafeApi.DependencyInjection;
public static class GnosisSafeApiServiceSetup
{
    public static IServiceCollection AddGnosisApi(this IServiceCollection services)
    {
        services.AddHttpClient<IGnosisSafeApiClient, GnosisSafeApiClient>((_, httpClient) =>
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        return services;
    }
}
