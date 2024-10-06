using SmartCoinOS.Domain.Deposit.Events;
using SmartCoinOS.Domain.Results;

namespace SmartCoinOS.Tests.Domain.DepositTests;

public sealed class DepositBankTests : BaseDepositTest
{
    [Fact]
    public void Archive_WhenIsDefault_FailsWithError()
    {
        var deposit = GetDefaultDepositBank();

        var archiveResult = deposit.Archive(UserInfo);
        var expectedError = archiveResult.HasError<EntityChangingStatusError>();

        Assert.True(expectedError);
    }

    [Fact]
    public void Archive_WhenIsArchived_FailsWithError()
    {
        var deposit = GetDepositBank();

        var archiveResult = deposit.Archive(UserInfo);
        var expectedSuccess = archiveResult.IsSuccess;

        var archiveAttempt = deposit.Archive(UserInfo);
        var expectedError = archiveAttempt.HasError<EntityAlreadyArchivedError>();

        Assert.True(expectedSuccess);
        Assert.True(expectedError);
    }

    [Fact]
    public void MakeDefault_WhenIsDefault_FailsWithError()
    {
        var deposit = GetDefaultDepositBank();

        var makeDefaultResult = deposit.MakeDefault(UserInfo);
        var expectedError = makeDefaultResult.HasError<EntityInvalidError>();

        Assert.True(expectedError);
    }

    [Fact]
    public void MakeDefault_WhenSucceeds_EmittsAuditEvent()
    {
        var deposit = GetDepositBank();

        var makeDefaultResult = deposit.MakeDefault(UserInfo);

        var expectedAuditEventFound = deposit.DomainEvents.Any(x => x is DepositBankMadeDefaultEvent);

        Assert.True(expectedAuditEventFound);
    }
}
