namespace SmartCoinOS.Domain.Orders;

public sealed record Instruction
{
    public required int Id { get; init; }
    public required string ReferenceNumber { get; init; }
}