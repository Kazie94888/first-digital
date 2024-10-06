using SmartCoinOS.Domain.Enums;
using SmartCoinOS.Domain.Orders;
using SmartCoinOS.Domain.Orders.Events;
using SmartCoinOS.Domain.Results;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Tests.Domain.OrderTests;

public sealed class RedeemOrderTests : BaseOrderTest
{
    [Fact]
    public void SetRedeemTransactionHash_WhenStatusNotRight_ReturnError()
    {
        var order = GetRedeemOrderFromFactory().EnrichWithRedeemHash();

        var result = order.SetTransactionHash(string.Empty, UserInfo);
        var hasStatusError = result.HasError<EntityChangingStatusError>();

        Assert.True(result.IsFailed);
        Assert.True(hasStatusError);
    }

    [Fact]
    public void SetRedeemTransactionHash_WhenOk_SetsProperties()
    {
        var order = GetRedeemOrderFromFactory();
        var sampleHash = "0x89af8c9f250426fc43ef4e77f35af6968176a8040b7956e4c7c415bae11dfdea";
        var setResult = order.SetTransactionHash(sampleHash, UserInfo);

        Assert.True(setResult.IsSuccess);
        Assert.Equal(OrderProcessingStatus.DepositTxHashSet, order.ProcessingStatus);
        Assert.Equal(sampleHash, order.RedeemTxHash);
        Assert.Contains(order.DomainEvents, x => x is SetTxHashAuditEvent);
    }

    [Fact]
    public void ConfirmRedeemDeposit_WhenStatusNotRight_FailsWithError()
    {
        var order = GetRedeemOrderFromFactory();

        var amount = new Money { Amount = 50_000, Currency = "FDUSD" };
        var result = order.ConfirmDeposit(amount, UserInfo);

        Assert.True(result.IsFailed);
        Assert.True(result.HasError<EntityChangingStatusError>());
    }

    [Fact]
    public void ConfirmRedeemDeposit_WhenOk_SetsProperties()
    {
        var order = GetRedeemOrderFromFactory().EnrichWithRedeemHash();

        var amount = new Money { Amount = 50_000, Currency = "FDUSD" };
        var result = order.ConfirmDeposit(amount, UserInfo);

        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.InProgress, order.Status);
        Assert.Equal(OrderProcessingStatus.TokenDepositConfirmed, order.ProcessingStatus);
        Assert.Equal(amount, order.DepositedAmount);
        Assert.Equal(amount, order.ActualAmount);
        Assert.Contains(order.DomainEvents, x => x is TokenDepositConfirmedEvent);
    }

    [Fact]
    public void RedeemExecuted_WhenStatusIsNotInProgress_FailsWithError()
    {
        var order = GetRedeemOrderFromFactory();

        var result = order.Executed(Signature, UserInfo);

        Assert.True(result.IsFailed);
        Assert.True(result.HasError<EntityInvalidOperation>());
    }

    [Fact]
    public void RedeemExecuted_WhenProcessingStatusIsNotSigningInitiated_FailsWithError()
    {
        var order = GetRedeemOrderFromFactory()
                    .EnrichWithRedeemHash()
                    .EnrichWithRedeemDepositConfirmation();

        var result = order.Executed(Signature, UserInfo);

        Assert.True(result.IsFailed);
        Assert.True(result.HasError<EntityInvalidOperation>());
    }

    [Fact]
    public void RedeemExecuted_WhenOk_SetsProperties()
    {
        var order = GetRedeemOrderFromFactory()
                    .EnrichWithRedeemHash()
                    .EnrichWithRedeemDepositConfirmation()
                    .EnrichWithSignature();
        var initialStatus = order.Status;

        var result = order.Executed(Signature, UserInfo);

        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.InProgress, initialStatus);
        Assert.Equal(OrderStatus.InProgress, order.Status);
        Assert.Equal(OrderProcessingStatus.TransactionExecuted, order.ProcessingStatus);
        Assert.Contains(order.DomainEvents, x => x is RedeemTransactionExecutedEvent);
    }

    [Fact]
    public void Complete_WhenStatusNotReady_FailsWithError()
    {
        var order = GetRedeemOrderFromFactory();

        var statusResult = order.Complete(UserInfo);

        order.EnrichWithRedeemHash();
        order.EnrichWithRedeemDepositConfirmation();

        var procStatusResult = order.Complete(UserInfo);

        Assert.True(statusResult.IsFailed);
        Assert.True(statusResult.HasError<EntityChangingStatusError>());
        Assert.True(procStatusResult.IsFailed);
        Assert.True(procStatusResult.HasError<EntityInvalidOperation>());
    }

    [Fact]
    public void Complete_WhenConditionsMet_SetsOrderStatus()
    {
        var order = GetRedeemOrderFromFactory()
                        .EnrichWithRedeemHash()
                        .EnrichWithRedeemDepositConfirmation()
                        .EnrichWithSignature()
                        .EnrichWithTransactionExecution()
                        .EnrichWithRsnReference();

        var completeResult = order.Complete(UserInfo);

        Assert.True(completeResult.IsSuccess);
        Assert.Equal(OrderStatus.Completed, order.Status);
        Assert.Equal(OrderProcessingStatus.WithdrawalConfirmed, order.ProcessingStatus);
    }

    [Fact]
    public void CancelRedeem_WhenOk_SetsOrderProperties()
    {
        var order = GetRedeemOrderFromFactory()
            .EnrichWithRedeemHash()
            .EnrichWithRedeemDepositConfirmation();

        var cancel = order.Cancel("Cancelled by user", UserInfo);

        Assert.True(cancel.IsSuccess);
        Assert.Equal(OrderStatus.Cancelled, order.Status);
        Assert.Equal(OrderProcessingStatus.Cancelled, order.ProcessingStatus);
        Assert.Contains(order.DomainEvents, x => x is RedeemOrderCancelledEvent);
    }

    [Fact]
    public void CancelRedeem_WhenNotBackOfficeUser_FailsWithError()
    {
        var order = GetRedeemOrderFromFactory();
        var userInfo = new UserInfo
        {
            Id = Guid.NewGuid(),
            Type = UserInfoType.ClientPortal,
            Username = "ClientPortal Tester"
        };

        var result = order.Cancel("Cancelled by non-backoffice user", userInfo);

        Assert.True(result.IsFailed);
        Assert.True(result.HasError<EntityInvalidOperation>());
    }

    [Fact]
    public void CancelRedeem_WhenSigningIsCompleted_FailsWithError()
    {
        var order = GetRedeemOrderFromFactory()
            .EnrichWithRedeemHash()
            .EnrichWithRedeemDepositConfirmation()
            .EnrichWithSignature();

        order.Executed(Signature, UserInfo);

        var result = order.Cancel("Cancelled by user", UserInfo);

        Assert.True(result.IsFailed);
        Assert.True(result.HasError<EntityInvalidOperation>());
    }
}
