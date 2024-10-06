using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Deposit.Events;
using SmartCoinOS.Domain.SeedWork;
using SmartCoinOS.Domain.Shared;
using StronglyTypedIds;

namespace SmartCoinOS.Domain.Deposit;

public sealed class DepositBank : AggregateRoot
{
    private DepositBank()
    {
    }

    public static DepositBank Create(string bankName, string beneficiary, string swift, string iban,
        AddressRecord address,
        bool isDefault, UserInfo createdBy)
    {
        var depositBank = new DepositBank
        {
            Id = DepositBankId.New(),
            Name = bankName,
            Beneficiary = beneficiary,
            Swift = swift,
            Iban = iban,
            CreatedBy = createdBy,
            Default = isDefault,
            Address = new Address
            {
                Id = AddressId.New(),
                Country = address.Country,
                City = address.City,
                PostalCode = address.PostalCode,
                Street = address.Street,
                State = address.State,
                CreatedBy = createdBy
            }
        };

        return depositBank;
    }

    public required DepositBankId Id { get; init; }
    public required string Name { get; init; }
    public required string Beneficiary { get; init; }
    public required string Swift { get; init; }
    public required string Iban { get; init; }
    public bool Default { get; private set; }
    public required Address Address { get; init; }
    public bool Archived { get; private set; }
    public DateTimeOffset ArchivedAt { get; private set; }

    public Result Archive(UserInfo userInfo)
    {
        if (Archived)
            return Result.Fail(new EntityAlreadyArchivedError(nameof(DepositBank)));

        if (Default)
            return Result.Fail(new EntityChangingStatusError(nameof(DepositBank), "Cannot archive default deposit."));

        Archived = true;
        ArchivedAt = DateTimeOffset.UtcNow;

        AddDomainEvent(new DepositBankArchivedEvent(Id, Name, Beneficiary, Iban, userInfo));

        return Result.Ok();
    }

    public Result MakeDefault(UserInfo userInfo)
    {
        if (Default)
            return Result.Fail(new EntityInvalidError(nameof(DepositBank), "Deposit is already default"));

        Default = true;

        AddDomainEvent(new DepositBankMadeDefaultEvent(Id, Name, Beneficiary, Iban, userInfo));

        return Result.Ok();
    }

    public void RemoveDefaultFlag()
    {
        Default = false;
    }
}

[StronglyTypedId]
public readonly partial struct DepositBankId;
