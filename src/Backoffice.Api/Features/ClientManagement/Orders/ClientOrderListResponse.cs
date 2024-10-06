using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Orders;

public sealed record ClientOrderListResponse
{
    public OrderId Id { get; init; }
    public required string PrettyId { get; init; }
    public OrderType OrderType { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public OrderStatus Progress { get; init; }
    public MoneyDto? Amount { get; init; }
    public required MoneyDto AmountOrdered { get; init; }
}