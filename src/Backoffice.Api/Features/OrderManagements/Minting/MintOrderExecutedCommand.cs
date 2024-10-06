using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Minting;

public sealed record MintOrderExecutedCommand : ICommand<EntityId>
{
    [FromRoute(Name = "orderId")]
    public required OrderId OrderId { get; init; }

    [FromBody]
    public required MintOrderExecutedRequest Request { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}

public sealed record MintOrderExecutedRequest
{
    public required SignatureDto Signature { get; init; }
}
