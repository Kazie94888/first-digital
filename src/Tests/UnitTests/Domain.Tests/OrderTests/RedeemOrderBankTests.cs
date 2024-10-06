using SmartCoinOS.Domain.Orders;
using SmartCoinOS.Domain.Orders.Events;

namespace SmartCoinOS.Tests.Domain.OrderTests;

public sealed class RedeemOrderBankTests : BaseOrderTest
{
    [Fact]
    public void RegisterPaymentInstruction_WhenConditionsMet_SetsProperties()
    {
        var instruction = new Instruction { Id = 1234, ReferenceNumber = "ABC-12341234" };
        var order = GetBankRedeemOrderFromFactory()
                    .EnrichWithRedeemHash()
                    .EnrichWithRedeemDepositConfirmation()
                    .EnrichWithSignature()
                    .EnrichWithTransactionExecution();

        var registerPmntResult = order.PaymentInstructionCreated(instruction.Id, instruction.ReferenceNumber);

        Assert.True(registerPmntResult.IsSuccess);
        Assert.Equal(OrderProcessingStatus.PaymentInstructionCreated, order.ProcessingStatus);
        Assert.Equal(instruction, order.PaymentInstruction);
    }

    [Fact]
    public void PaymentInstructionCompleted_WhenConditionsMet_SetsProperties()
    {
        var instruction = new Instruction { Id = 1234, ReferenceNumber = "ABC-12341234" };
        var order = GetBankRedeemOrderFromFactory()
                    .EnrichWithRedeemHash()
                    .EnrichWithRedeemDepositConfirmation()
                    .EnrichWithSignature()
                    .EnrichWithTransactionExecution();

        var registerPmntResult = order.PaymentInstructionCreated(instruction.Id, instruction.ReferenceNumber);
        var completePmntResult = order.PaymentInstructionCompleted(UserInfo);
        var paymentCompletedEvntExists = order.DomainEvents.Any(x => x is RedeemPaymentInstructionConfirmedEvent);

        Assert.True(registerPmntResult.IsSuccess);
        Assert.True(completePmntResult.IsSuccess);
        Assert.Equal(OrderProcessingStatus.PaymentInstructionCompleted, order.ProcessingStatus);
        Assert.True(paymentCompletedEvntExists);
    }
}