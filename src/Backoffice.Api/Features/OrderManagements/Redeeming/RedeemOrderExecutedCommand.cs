using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Redeeming;

public sealed record RedeemOrderExecutedCommand : ICommand<EntityId>
{
    [FromRoute(Name = "orderId")]
    public required OrderId OrderId { get; init; }

    [FromBody]
    public required RedeemOrderExecutionRequest Request { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}

public sealed record RedeemOrderExecutionRequest
{
    public required SignatureDto Signature { get; init; }
}
