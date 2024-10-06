using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.ClientPortal.Api.Features.OrderInsights.Details;

internal sealed record MintOrderDetailsQuery : IQuery<MintOrderDetailsResponse>
{
    [FromRoute(Name = "orderId")] 
    public required OrderId OrderId { get; init; }

    public required UserInfoDto UserInfo { get; init; }
}
