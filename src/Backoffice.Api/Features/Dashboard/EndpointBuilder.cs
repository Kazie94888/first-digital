using MediatR;
using SmartCoinOS.Backoffice.Api.Features.Dashboard.Overview;
using SmartCoinOS.Extensions;

namespace SmartCoinOS.Backoffice.Api.Features.Dashboard;

internal class EndpointBuilder : IEndpointBuilder
{
    public void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/dashboard/overview",
            async (IMediator mediator,
                [AsParameters] OverviewQuery query,
                CancellationToken cancellationToken) =>
            {
                var overview = await mediator.Send(query, cancellationToken);
                return overview.ToHttpResult();
            });
    }
}
