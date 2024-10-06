using MediatR;
using SmartCoinOS.ClientPortal.Api.Extensions;
using SmartCoinOS.ClientPortal.Api.Features.OrderManagements.Create;
using SmartCoinOS.ClientPortal.Api.Features.OrderManagements.Minting;
using SmartCoinOS.ClientPortal.Api.Features.OrderManagements.Orders;
using SmartCoinOS.ClientPortal.Api.Features.OrderManagements.Redeeming;
using SmartCoinOS.Extensions;

namespace SmartCoinOS.ClientPortal.Api.Features.OrderManagements;

internal sealed class EndpointBuilder : IEndpointBuilder
{
    public void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("/orders/mint", async (
            IMediator mediator,
            [AsParameters] MintOrderCommand mintOrderCommand,
            CancellationToken cancellationToken
        ) =>
        {
            var order = await mediator.Send(mintOrderCommand, cancellationToken);
            return order.ToHttpResult();
        }).RequireAuthorization();

        routeBuilder.MapPost("/orders/redeem", async (
            IMediator mediator,
            [AsParameters] RedeemOrderCommand query,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();

        routeBuilder.MapGet("/orders/clients/resources", async (
            IMediator mediator,
            [AsParameters] CreateOrderClientResourcesQuery clientResourcesQuery,
            CancellationToken cancellationToken = default) =>
        {
            var resources = await mediator.Send(clientResourcesQuery, cancellationToken);
            return resources.ToHttpResult();
        }).RequireAuthorization();

        routeBuilder.MapGet("/orders/{orderId}/audits", async (
            IMediator mediator,
            [AsParameters] DetailedOrderAuditListQuery query,
            CancellationToken cancellationToken = default) =>
        {
            var audits = await mediator.Send(query, cancellationToken);
            return audits.ToHttpResult();
        }).RequireAuthorization();
    }
}
