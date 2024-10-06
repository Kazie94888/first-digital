using SmartCoinOS.ClientPortal.Api.Base.Dto;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.ClientPortal.Api.Features.Dashboard.Orders;

internal sealed record OrderListResponse
{
    public OrderId Id { get; init; }
    public required string PrettyId { get; init; }
    public OrderType OrderType { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public OrderStatus Progress { get; init; }
    public required MoneyDto OrderAmount { get; init; }
    public required UserInfoDto CreatedBy { get; init; }
}
