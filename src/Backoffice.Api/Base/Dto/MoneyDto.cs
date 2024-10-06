using SmartCoinOS.Domain.Orders;

namespace SmartCoinOS.Backoffice.Api.Base.Dto;

public sealed record MoneyDto
{
    public required decimal Amount { get; init; }
    public required string AssetSymbol { get; init; }

    public static implicit operator Money(MoneyDto dtoMoney) =>
        new() { Amount = dtoMoney.Amount, Currency = dtoMoney.AssetSymbol };
}
