using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Deposit;
using SmartCoinOS.Domain.Exceptions;
using SmartCoinOS.Domain.Orders.Events;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Domain.Orders;

public sealed class MintOrder : Order
{
    private MintOrder() { }

    internal static MintOrder Create(ClientId client, OrderNumber orderNumber, Money amount,
        WalletId clientWallet, DepositWalletId? depositWallet, UserInfo createdBy)
    {
        var order = new MintOrder
        {
            Id = OrderId.New(),
            OrderNumber = orderNumber,
            Type = OrderType.Mint,
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

    public Result CreateDepositInstruction(int depositInstructionId, string referenceNumber)
    {
        EntityNullException.ThrowIfNull(BankAccountId, nameof(FdtAccount));

        if (Type is not OrderType.Mint || DepositInstruction is not null)
            return Result.Fail(new EntityInvalidError(nameof(DepositInstruction)));

        ProcessingStatus = OrderProcessingStatus.DepositInstructionCreated;
        DepositInstruction = new Instruction
        {
            Id = depositInstructionId,
            ReferenceNumber = referenceNumber,
        };

        return Result.Ok();
    }

    public Result CompleteDepositInstruction(IReadOnlyList<Money> instructionTransactions, UserInfo userInfo)
    {
        if (ProcessingStatus is not OrderProcessingStatus.DepositInstructionCreated)
            return Result.Fail(new EntityInvalidOperation(nameof(DepositInstruction)));

        var amountToMint = instructionTransactions.Sum(x => x.Amount);
        if (amountToMint <= 0m)
            return Result.Fail(new EntityInvalidOperation(nameof(DepositInstruction), $"Total of transactions results in invalid amount of '{amountToMint}'"));

        ActualAmount = new Money { Amount = amountToMint, Currency = OrderedAmount.Currency };
        DepositedAmount = new Money { Amount = amountToMint, Currency = instructionTransactions[0].Currency };
        ProcessingStatus = OrderProcessingStatus.DepositInstructionCompleted;
        OrderInProgress();

        AddDomainEvent(new OrderDepositConfirmedEvent(Id, ClientId, userInfo));

        return Result.Ok();
    }

    public Result AddRsnAmount(Money receivedAmount, UserInfo userInfo)
    {
        if (!FdtAccountId.HasValue)
            return Result.Fail(new EntityInvalidError(nameof(Order)));

        if (ProcessingStatus is not OrderProcessingStatus.RsnReferenceCreated)
            return Result.Fail(new EntityChangingStatusError(nameof(Order)));

        ActualAmount = receivedAmount with { Currency = OrderedAmount.Currency };
        DepositedAmount = receivedAmount;
        ProcessingStatus = OrderProcessingStatus.RsnAmountAdded;
        OrderInProgress();

        AddDomainEvent(new OrderRsnAmountAddedEvent(Id, ClientId, userInfo));

        return Result.Ok();
    }

    public override Result Signed(string safeTxHash, List<SafeSignature> signatures, UserInfo userInfo)
    {
        if (ProcessingStatus is not (OrderProcessingStatus.SigningInitiated or OrderProcessingStatus.RsnAmountAdded or OrderProcessingStatus.DepositInstructionCompleted))
            return Result.Fail(new EntityChangingStatusError(nameof(Order), "Order status doesn't allow signatures"));

        return base.Signed(safeTxHash, signatures, userInfo);
    }

    public override Result AddRsnReference(RsnReference rsnReference, UserInfo userInfo)
    {
        if (ProcessingStatus is not null)
            return Result.Fail(new EntityChangingStatusError(nameof(Order), "Rsn reference can not be added at this phase."));

        return base.AddRsnReference(rsnReference, userInfo);
    }

    public override Result Executed(SafeSignature signature, UserInfo userInfo)
    {
        OrderCompleted();
        AddDomainEvent(new MintTransactionExecutedEvent(Id, signature, ClientId, userInfo));

        return TransactionExecuted(signature);
    }
}
