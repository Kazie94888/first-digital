using SmartCoinOS.Domain.Deposit.Events;
using SmartCoinOS.Domain.SeedWork;
using StronglyTypedIds;

namespace SmartCoinOS.Domain.Deposit;

public sealed class DepositFdtAccount : AggregateRoot
{
    private DepositFdtAccount()
    {
    }

    public static DepositFdtAccount Create(string accountName, FdtDepositAccountNumber accountNumber, UserInfo createdBy)
    {
        var fdtDepositAccount = new DepositFdtAccount
        {
            Id = DepositFdtAccountId.New(),
            AccountName = accountName,
            AccountNumber = accountNumber,
            CreatedBy = createdBy,
        };

        fdtDepositAccount.AddDomainEvent(new FdtDepositAccountCreatedEvent(fdtDepositAccount.Id, accountName, accountNumber, createdBy));

        return fdtDepositAccount;
    }

    public required DepositFdtAccountId Id { get; init; }
    public required string AccountName { get; init; }
    public required FdtDepositAccountNumber AccountNumber { get; init; }

    public bool Archived { get; private set; }
    public DateTimeOffset? ArchivedAt { get; private set; }

    public Result Archive(UserInfo userInfo)
    {
        if (Archived)
            return new EntityAlreadyArchivedError(nameof(DepositFdtAccount), "Deposit FDT Account is already archived.");

        Archived = true;
        ArchivedAt = DateTimeOffset.UtcNow;

        AddDomainEvent(new FdtDepositAccountArchivedEvent(Id, AccountNumber, userInfo));

        return Result.Ok();
    }
}

public sealed record FdtDepositAccountNumber(string Value)
{
    public static implicit operator string(FdtDepositAccountNumber fdtDepositAccountNumber) => fdtDepositAccountNumber.Value;
}

[StronglyTypedId]
public readonly partial struct DepositFdtAccountId;
