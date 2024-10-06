using MediatR;
using SmartCoinOS.ClientPortal.Api.Extensions;
using SmartCoinOS.ClientPortal.Api.Features.OrderInsights.Details;
using SmartCoinOS.Extensions;

namespace SmartCoinOS.ClientPortal.Api.Features.OrderInsights;

internal sealed class EndpointBuilder : IEndpointBuilder
{
    public void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/orders/{orderId}/mint", async (
            IMediator mediator,
            [AsParameters] MintOrderDetailsQuery query,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();

        routeBuilder.MapGet("/orders/{orderId}/redeem", async (
            IMediator mediator,
            [AsParameters] RedeemOrderDetailsQuery query,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();
    }
}
