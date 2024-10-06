using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SmartCoinOS.Integrations.CourierApi.Contracts;
using SmartCoinOS.Integrations.CourierApi.Services;

namespace SmartCoinOS.Integrations.CourierApi.DependencyInjection;

public static class CourierApiServiceSetup
{
    public static void AddCourierApi(this IServiceCollection services, Action<CourierSettings> config)
    {
        services.Configure(config);

        services.AddHttpClient<ICourierApiClient, CourierApiClient>((serviceProvider, httpClient) =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<CourierSettings>>().Value;

            httpClient.BaseAddress = new Uri(settings.ApiUrl);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey);
        });
    }
}
