using Microsoft.AspNetCore.Mvc;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Orders;

internal sealed record MintOrderDetailsQuery : IQuery<MintOrderDetailsResponse>
{
    [FromRoute(Name = "orderId")]
    public required OrderId OrderId { get; init; }
}