using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Deposit;
using SmartCoinOS.Domain.Enums;
using SmartCoinOS.Domain.Orders.Events;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Orders;

public sealed class RedeemOrder : Order
{
    private RedeemOrder() { }

    internal static RedeemOrder Create(ClientId client, OrderNumber orderNumber, Money amount,
    WalletId clientWallet, DepositWalletId? depositWallet, UserInfo createdBy)
    {
        var order = new RedeemOrder
        {
            Id = OrderId.New(),
            OrderNumber = orderNumber,
            Type = OrderType.Redeem,
            WalletId = clientWallet,
            OrderedAmount = amount,
            DepositWalletId = depositWallet,
            ClientId = client,
            CreatedBy = createdBy,
            Status = OrderStatus.Initiated
        };

        order.AddDomainEvent(new OrderCreatedEvent(order.Id, order.OrderNumber, clientWallet, order.Type, client, createdBy));

        return order;
    }

    public Result SetTransactionHash(string redeemTxHash, UserInfo userInfo)
    {
        if (ProcessingStatus is not null)
            return Result.Fail(new EntityChangingStatusError(nameof(ProcessingStatus), "Order status doesn't allow further hash modifications"));

        ProcessingStatus = OrderProcessingStatus.DepositTxHashSet;
        RedeemTxHash = redeemTxHash;

        AddDomainEvent(new SetTxHashAuditEvent(Id, redeemTxHash, ClientId, userInfo));
        UpgradeVersion();

        return Result.Ok();
    }

    public Result ConfirmDeposit(Money depositedAmount, UserInfo userInfo)
    {
        if (ProcessingStatus is not OrderProcessingStatus.DepositTxHashSet)
            return Result.Fail(new EntityChangingStatusError(nameof(ProcessingStatus), "Order status doesn't allow confirmation at this point."));

        if (depositedAmount.Amount <= 0m)
            return Result.Fail(new EntityInvalidOperation(nameof(ConfirmDeposit), $"Invalid deposit amount ({depositedAmount.Amount} {depositedAmount.Currency})"));

        ProcessingStatus = OrderProcessingStatus.TokenDepositConfirmed;
        DepositedAmount = depositedAmount;
        ActualAmount = new Money { Amount = depositedAmount.Amount, Currency = OrderedAmount.Currency };
        OrderInProgress();

        AddDomainEvent(new TokenDepositConfirmedEvent(Id, depositedAmount, ClientId, userInfo));

        return Result.Ok();
    }

    public Result PaymentInstructionCreated(int instructionId, string referenceNumber)
    {
        if (Type is not OrderType.Redeem || !IsBankTransfer())
            return Result.Fail(new EntityInvalidError(nameof(Order), "Only redeem orders made via bank transfer can complete a payment instruction"));

        if (ProcessingStatus is not OrderProcessingStatus.TransactionExecuted || PaymentInstruction is not null)
            return Result.Fail(new EntityInvalidOperation(nameof(DepositInstruction)));

        ProcessingStatus = OrderProcessingStatus.PaymentInstructionCreated;
        PaymentInstruction = new Instruction
        {
            Id = instructionId,
            ReferenceNumber = referenceNumber
        };

        return Result.Ok();
    }

    public Result PaymentInstructionCompleted(UserInfo userInfo)
    {
        if (!IsBankTransfer())
            return Result.Fail(new EntityInvalidError(nameof(Order), "Only orders made via bank transfer can complete a payment instruction"));

        if (ProcessingStatus is not OrderProcessingStatus.PaymentInstructionCreated)
            return Result.Fail(new EntityChangingStatusError(nameof(Order), "Order status doesn't allow completing payment instruction"));

        ProcessingStatus = OrderProcessingStatus.PaymentInstructionCompleted;

        AddDomainEvent(new RedeemPaymentInstructionConfirmedEvent(Id, ClientId, userInfo));

        return Result.Ok();
    }

    public override Result Signed(string safeTxHash, List<SafeSignature> signatures, UserInfo userInfo)
    {
        if (ProcessingStatus is not (OrderProcessingStatus.TokenDepositConfirmed or OrderProcessingStatus.SigningInitiated))
            return Result.Fail(new EntityChangingStatusError(nameof(Order), "Order status doesn't allow signatures"));

        return base.Signed(safeTxHash, signatures, userInfo);
    }

    public Result Cancel(string reason, UserInfo userInfo)
    {
        if (userInfo.Type != UserInfoType.BackOffice)
            return Result.Fail(new EntityInvalidOperation(nameof(Order), "Only BackOffice users can cancel Redeem orders."));

        if (ProcessingStatus is not (null
                                    or OrderProcessingStatus.DepositTxHashSet
                                    or OrderProcessingStatus.TokenDepositConfirmed
                                    or OrderProcessingStatus.SigningInitiated))
            return Result.Fail(new EntityInvalidOperation(nameof(Order),
                $"Order processing status '{Status}' not permitted for cancellation."));

        ProcessingStatus = OrderProcessingStatus.Cancelled;
        OrderCanceled();

        AddDomainEvent(new RedeemOrderCancelledEvent(Id, OrderNumber, reason, ClientId, userInfo));

        return Result.Ok();
    }

    public override Result AddRsnReference(RsnReference rsnReference, UserInfo userInfo)
    {
        if (ProcessingStatus is not OrderProcessingStatus.TransactionExecuted)
            return Result.Fail(new EntityChangingStatusError(nameof(Order), "Rsn reference can only be added after transaction is executed"));

        return base.AddRsnReference(rsnReference, userInfo);
    }

    public Result Complete(UserInfo userInfo)
    {
        if (Status is not OrderStatus.InProgress)
            return Result.Fail(new EntityChangingStatusError(nameof(Order), "Order status doesn't allow completion"));

        var allowedPreviousStatuses = new OrderProcessingStatus[] { OrderProcessingStatus.RsnReferenceCreated, OrderProcessingStatus.PaymentInstructionCompleted };
        if (!Array.Exists(allowedPreviousStatuses, x => x == ProcessingStatus))
            return Result.Fail(new EntityInvalidOperation(nameof(Order), "Processing status doesn't allow completion"));

        ProcessingStatus = OrderProcessingStatus.WithdrawalConfirmed;

        OrderCompleted();
        AddDomainEvent(new RedeemOrderCompletedEvent(Id, ClientId, userInfo));

        return Result.Ok();
    }

    public override Result Executed(SafeSignature signature, UserInfo userInfo)
    {
        AddDomainEvent(new RedeemTransactionExecutedEvent(Id, signature, ClientId, userInfo));
        return TransactionExecuted(signature);
    }
}