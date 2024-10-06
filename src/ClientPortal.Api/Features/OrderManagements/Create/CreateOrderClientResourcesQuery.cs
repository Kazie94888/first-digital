using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.ClientPortal.Api.Features.OrderManagements.Create;

internal sealed record CreateOrderClientResourcesQuery : IQuery<CreateOrderClientResourcesResponse>
{
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
    
    public required UserInfoDto UserInfo { get; init; }
}