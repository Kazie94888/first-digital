using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Deposit.Events;
using SmartCoinOS.Domain.SeedWork;
using StronglyTypedIds;

namespace SmartCoinOS.Domain.Deposit;

public sealed class DepositWallet : AggregateRoot
{
    private DepositWallet() { }

    public static DepositWallet Create(WalletAccount account, bool isDefault, UserInfo createdBy)
    {
        var depositWallet = new DepositWallet
        {
            Id = DepositWalletId.New(),
            Account = account,
            Default = isDefault,
            CreatedBy = createdBy
        };

        return depositWallet;
    }

    public required DepositWalletId Id { get; init; }
    public required WalletAccount Account { get; init; }
    public bool Default { get; private set; }

    public Result MakeDefault(UserInfo userInfo)
    {
        if (Default)
            return Result.Fail(new EntityInvalidError(nameof(DepositWallet), "Wallet is already default"));

        Default = true;

        AddDomainEvent(new DepositWalletMadeDefaultEvent(Id, Account, userInfo));

        return Result.Ok();
    }

    public void RemoveDefaultFlag()
    {
        Default = false;
    }
}

[StronglyTypedId]
public readonly partial struct DepositWalletId;