using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Redeeming;

public sealed record RedeemOrderSetTxHashCommand : ICommand<EntityId>
{
    [FromRoute(Name = "orderId")]
    public required OrderId OrderId { get; init; }

    [FromBody]
    public required RedeemOrderSetTxHash Request { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}

public sealed record RedeemOrderSetTxHash
{
    public required string TxHash { get; init; }
}
