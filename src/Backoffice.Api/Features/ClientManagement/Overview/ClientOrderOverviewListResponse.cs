using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Overview;

public sealed record ClientOrderOverviewListResponse
{
    public OrderId Id { get; init; }
    public required string PrettyId { get; init; }
    public OrderType OrderType { get; init; }
    public OrderStatus Status { get; init; }
    public OrderProcessingStatus? ProcessingStatus { get; init; }
    public MoneyDto? MintedBurned { get; init; }
    public MoneyDto? Ordered { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
