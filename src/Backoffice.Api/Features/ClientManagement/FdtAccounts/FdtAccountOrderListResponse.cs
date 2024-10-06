using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.FdtAccounts;

public sealed record FdtAccountOrderListResponse
{
    public required OrderId Id { get; init; }
    public required string PrettyId { get; init; }
    public required OrderType OrderType { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required string ClientName { get; init; }
    public required OrderStatus Progress { get; init; }
    public OrderProcessingStatus? SubStatus { get; init; }
    public MoneyDto? Amount { get; init; }
    public required MoneyDto AmountOrdered { get; init; }
}
