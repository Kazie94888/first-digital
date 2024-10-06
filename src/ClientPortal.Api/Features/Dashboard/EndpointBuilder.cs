using MediatR;
using SmartCoinOS.ClientPortal.Api.Extensions;
using SmartCoinOS.ClientPortal.Api.Features.Dashboard.Orders;
using SmartCoinOS.Extensions;

namespace SmartCoinOS.ClientPortal.Api.Features.Dashboard;

internal sealed class EndpointBuilder : IEndpointBuilder
{
    public void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/dashboard/orders", async (
            IMediator mediator,
            [AsParameters] OrderListQuery query,
            CancellationToken cancellationToken
        ) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();
    }
}
