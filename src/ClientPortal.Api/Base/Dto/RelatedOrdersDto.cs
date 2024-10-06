using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.ClientPortal.Api.Base.Dto;

internal sealed record RelatedOrdersDto
{
    public required OrderId Id { get; init; }
    public required OrderType Type { get; init; }
    public required string PrettyId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required OrderStatus Status { get; init; }
    public required MoneyDto Amount { get; init; }
}
