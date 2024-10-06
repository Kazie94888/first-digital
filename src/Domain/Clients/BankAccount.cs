using SmartCoinOS.Domain.Exceptions;
using SmartCoinOS.Domain.Shared;
using StronglyTypedIds;

namespace SmartCoinOS.Domain.Clients;

public sealed class BankAccount : ClientAsset
{
    internal BankAccount()
    {
    }

    public required BankAccountId Id { get; init; }
    public required string Beneficiary { get; init; }
    public required string Iban { get; init; }
    public required string BankName { get; init; }
    public required string SwiftCode { get; init; }
    public string? Alias { get; private set; }
    public required Address Address { get; init; }
    public required ClientId ClientId { get; init; }

    public required SmartTrustBank SmartTrustBank { get; init; }

    internal void SetAlias(string alias)
    {
        Alias = alias;
    }

    public int GetIncomingSmartTrustBank()
    {
        return SmartTrustBank.OwnBankId
                ?? throw new EntityNullException($"{nameof(SmartTrustBank.OwnBankId)} not found for bank {Id}");
    }

    public int GetOutgoingSmartTrustBank()
    {
        return SmartTrustBank.ThirdPartyBankId
            ?? throw new EntityNullException($"{nameof(SmartTrustBank.ThirdPartyBankId)} not found for bank {Id}");
    }
}

[StronglyTypedId]
public readonly partial struct BankAccountId;
