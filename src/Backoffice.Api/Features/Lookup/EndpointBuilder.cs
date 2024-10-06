using SmartCoinOS.Domain.Base;
using SmartCoinOS.Extensions;

namespace SmartCoinOS.Backoffice.Api.Features.Lookup;

public class EndpointBuilder : IEndpointBuilder
{
    public void Map(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/lookup/countries",
            async () => await Task.FromResult(GlobalConstants.Country.SupportedCountries())).CacheOutput();
    }
}
