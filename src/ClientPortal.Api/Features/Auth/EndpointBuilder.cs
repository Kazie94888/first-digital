using MediatR;
using SmartCoinOS.ClientPortal.Api.Extensions;
using SmartCoinOS.ClientPortal.Api.Features.Auth.Info;
using SmartCoinOS.Extensions;

namespace SmartCoinOS.ClientPortal.Api.Features.Auth;

internal sealed class EndpointBuilder : IEndpointBuilder
{
    public void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/auth/info", async (
            IMediator mediator,
            [AsParameters] AuthInfoQuery query,
            CancellationToken cancellationToken
            ) =>
        {
            var result = await mediator.Send(query, cancellationToken);
            return result.ToHttpResult();
        }).RequireAuthorization();
    }
}
