using StronglyTypedIds;

namespace SmartCoinOS.Domain.Clients;

public sealed class FdtAccount : ClientAsset
{
    internal FdtAccount() { }

    public required FdtAccountId Id { get; init; }
    public required string ClientName { get; init; }
    public required FdtAccountNumber AccountNumber { get; init; }
    public string? Alias { get; init; }
    public required ClientId ClientId { get; init; }
}

public sealed record FdtAccountNumber(string Value)
{
    public static implicit operator string(FdtAccountNumber fdtAccountNumber) => fdtAccountNumber.Value;
}


[StronglyTypedId]
public readonly partial struct FdtAccountId;