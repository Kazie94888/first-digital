using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Backoffice.Api.Features.OrderManagements.Orders;

public sealed record OrderListResponse
{
    public OrderId Id { get; init; }
    public required string PrettyId { get; init; }
    public OrderType OrderType { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public required string ClientName { get; init; }
    public OrderStatus Progress { get; init; }
    public OrderProcessingStatus? ProcessingStatus { get; init; }
    public MoneyDto? Amount { get; init; }
    public required MoneyDto AmountOrdered { get; init; }
}
