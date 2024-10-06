using SmartCoinOS.Backoffice.Api.Base.WebHooks;
using SmartCoinOS.Backoffice.Api.Features.Webhooks.SmartTrust;
using SmartCoinOS.Extensions;

namespace SmartCoinOS.Backoffice.Api.Features.Webhooks;

internal class EndpointBuilder : IEndpointBuilder
{
    public void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapWebHook<SmartTrustWebhook>("/smart-trust");
    }
}
