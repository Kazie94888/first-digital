using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.ClientPortal.Api.Base.Dto;

internal sealed record MoneyDto
{
    public required decimal Amount { get; init; }
    public required string AssetSymbol { get; init; }

    public static implicit operator Money(MoneyDto dtoMoney) =>
        new()
        {
            Amount = dtoMoney.Amount,
            Currency = dtoMoney.AssetSymbol
        };

    public static implicit operator MoneyDto(Money money) =>
        new()
        {
            Amount = money.Amount,
            AssetSymbol = money.Currency
        };
}
