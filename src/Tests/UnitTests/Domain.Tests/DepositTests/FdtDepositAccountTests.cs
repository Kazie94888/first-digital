using SmartCoinOS.Domain.Deposit.Events;
using SmartCoinOS.Domain.Results;

namespace SmartCoinOS.Tests.Domain.DepositTests;

public class FdtDepositAccountTests : BaseDepositTest
{
    [Fact]
    public void CreateFdtDepositAccount_WithValidData_Successfully()
    {
        var fdtDepositAccount = GetFdtDepositAccountFromFactory();

        Assert.NotNull(fdtDepositAccount);
        Assert.Equal(FdtDepositAccountName, fdtDepositAccount.AccountName);
        Assert.Equal(FdtDepositAccountNumber, fdtDepositAccount.AccountNumber.Value);
        Assert.False(fdtDepositAccount.Archived);
        Assert.Null(fdtDepositAccount.ArchivedAt);
    }

    [Fact]
    public void CreateFdtDepositAccount_WhenSuccess_FdtDepositAccountCreatedEventIsRegistered()
    {
        var fdtDepositAccount = GetFdtDepositAccountFromFactory();
        Assert.NotNull(fdtDepositAccount);

        var eventEmitted = fdtDepositAccount.DomainEvents.Any(x => x is FdtDepositAccountCreatedEvent);
        Assert.True(eventEmitted);
    }

    [Fact]
    public void ArchiveFdtDepositAccount_WhenSuccess_ArchivedIsTrue()
    {
        var fdtDepositAccount = GetFdtDepositAccountFromFactory();
        var result = fdtDepositAccount.Archive(UserInfo);

        Assert.True(result.IsSuccess);
        Assert.True(fdtDepositAccount.Archived);
        Assert.NotNull(fdtDepositAccount.ArchivedAt);
    }

    [Fact]
    public void ArchiveFdtDepositAccount_WhenAlreadyArchived_ThrowException()
    {
        var fdtDepositAccount = GetFdtDepositAccountFromFactory();
        fdtDepositAccount.Archive(UserInfo);

        var result = fdtDepositAccount.Archive(UserInfo);

        Assert.True(result.IsFailed);
        Assert.True(result.HasError<EntityAlreadyArchivedError>());
    }

    [Fact]
    public void ArchiveFdtDepositAccount_WhenSuccess_FdtDepositAccountArchivedEventIsRegistered()
    {
        var fdtDepositAccount = GetFdtDepositAccountFromFactory();
        fdtDepositAccount.Archive(UserInfo);

        var eventEmitted = fdtDepositAccount.DomainEvents.Any(x => x is FdtDepositAccountArchivedEvent);
        Assert.True(eventEmitted);
    }
}