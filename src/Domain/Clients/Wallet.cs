using SmartCoinOS.Domain.Exceptions;
using StronglyTypedIds;

namespace SmartCoinOS.Domain.Clients;

public sealed class Wallet : ClientAsset
{
    internal Wallet()
    {
    }

    public required WalletId Id { get; init; }
    public required WalletAccount WalletAccount { get; init; }
    public string? Alias { get; internal set; }
    public int OrdersCount { get; private set; }

    public required ClientId ClientId { get; init; }

    internal void SetOrderCount(int orderCount)
    {
        if (Archived)
            throw new EntityInvalidStateException("Archived wallet can't be used in new orders.");

        OrdersCount = orderCount;
    }
}

[StronglyTypedId]
public readonly partial struct WalletId;
