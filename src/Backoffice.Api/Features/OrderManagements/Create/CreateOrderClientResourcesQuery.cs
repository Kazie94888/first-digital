using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Create;

internal sealed record CreateOrderClientResourcesQuery : IQuery<CreateOrderClientResourcesResponse>
{
    [FromRoute(Name = "clientId")]
    public required ClientId ClientId { get; init; }

    [FromQuery(Name = "type")]
    public required string Type { get; init; }

    public OrderType OrderType()
    {
        return Type switch
        {
            "mint" => Domain.Orders.OrderType.Mint,
            "redeem" => Domain.Orders.OrderType.Redeem,
            _ => throw new ArgumentOutOfRangeException(nameof(Type)),
        };
    }
}
