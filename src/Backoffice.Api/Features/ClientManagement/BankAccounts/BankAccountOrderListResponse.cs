using SmartCoinOS.Backoffice.Api.Base.Dto;
using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.BankAccounts;

public sealed record BankAccountOrderListResponse
{
    public required OrderId Id { get; init; }
    public required string PrettyId { get; init; }
    public required OrderType OrderType { get; init; }
    public required OrderStatus Status { get; init; }
    public OrderProcessingStatus? ProcessingStatus { get; init; }
    public MoneyDto? MintedBurned { get; init; }
    public required MoneyDto Ordered { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}