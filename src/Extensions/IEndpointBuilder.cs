using Microsoft.AspNetCore.Routing;

namespace SmartCoinOS.Extensions;

public interface IEndpointBuilder
{
    void Map(IEndpointRouteBuilder routeBuilder);
}
