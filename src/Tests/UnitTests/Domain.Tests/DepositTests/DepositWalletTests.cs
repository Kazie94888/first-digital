using SmartCoinOS.Domain.Deposit.Events;
using SmartCoinOS.Domain.Results;

namespace SmartCoinOS.Tests.Domain.DepositTests;

public sealed class DepositWalletTests : BaseDepositTest
{
    [Fact]
    public void MakeDefault_WhenIsDefault_ReturnsError()
    {
        var wallet = GetDefaultDepositWallet();

        var makeDefaultResult = wallet.MakeDefault(UserInfo);
        var expectedError = makeDefaultResult.HasError<EntityInvalidError>();

        Assert.True(expectedError);
    }


    [Fact]
    public void MakeDefault_WhenSucceeds_EmitsAuditEvent()
    {
        var deposit = GetDepositWallet();

        deposit.MakeDefault(UserInfo);

        var expectedAuditEventFound = deposit.DomainEvents.Any(x => x is DepositWalletMadeDefaultEvent);

        Assert.True(expectedAuditEventFound);
    }
}