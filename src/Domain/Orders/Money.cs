namespace SmartCoinOS.Domain.Orders;

public sealed record Money
{
    public decimal Amount { get; init; }
    public required string Currency { get; init; }
}