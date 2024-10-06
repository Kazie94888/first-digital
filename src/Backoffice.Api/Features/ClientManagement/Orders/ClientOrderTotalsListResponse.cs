using SmartCoinOS.Backoffice.Api.Base.Dto;

namespace SmartCoinOS.Backoffice.Api.Features.ClientManagement.Orders;

public sealed record ClientOrderTotalsListResponse
{
    public required MoneyDto MintInProgress { get; init; }
    public required MoneyDto MintCompleted { get; init; }
    public required MoneyDto RedeemInProgress { get; init; }
    public required MoneyDto RedeemCompleted { get; init; }
}
