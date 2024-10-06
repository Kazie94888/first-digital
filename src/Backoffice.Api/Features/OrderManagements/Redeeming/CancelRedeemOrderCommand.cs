using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Redeeming;

public class CancelRedeemOrderCommand : ICommand<EntityId>
{
    [FromRoute(Name = "orderId")]
    public required OrderId OrderId { get; init; }

    [FromBody]
    public required CancelRedeemOrderRequest Request { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}

public sealed record CancelRedeemOrderRequest
{
    public required string Reason { get; init; }
}
