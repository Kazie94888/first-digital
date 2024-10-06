namespace SmartCoinOS.Domain.Orders;

public sealed record SafeSignature(string Address, string? Alias, DateTimeOffset SubmissionDate);
