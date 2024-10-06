using SmartCoinOS.Domain.Clients;
using SmartCoinOS.Domain.Deposit;
using SmartCoinOS.Domain.Exceptions;
using SmartCoinOS.Domain.Orders.Events;
using SmartCoinOS.Domain.SeedWork;
using StronglyTypedIds;

namespace SmartCoinOS.Domain.Orders;

public abstract class Order : AggregateRoot
{
    protected Order() { }

    internal void SetPaymentMode(FdtAccountId clientFdtAccount, DepositFdtAccountId depositFdtAccount)
    {
        FdtAccountId = clientFdtAccount;
        DepositFdtAccountId = depositFdtAccount;
    }

    internal void SetPaymentMode(BankAccountId bankId, DepositBankId depositBankId)
    {
        BankAccountId = bankId;
        DepositBankId = depositBankId;
    }

    public required OrderId Id { get; init; }
    public required OrderNumber OrderNumber { get; init; }
    public required OrderType Type { get; init; }
    public OrderStatus Status { get; protected set; }
    public OrderProcessingStatus? ProcessingStatus { get; protected set; }
    public required Money OrderedAmount { get; init; }
    public Money? DepositedAmount { get; protected set; }
    public Money? ActualAmount { get; protected set; }
    public required ClientId ClientId { get; init; }
    public required WalletId WalletId { get; init; }
    public BankAccountId? BankAccountId { get; private set; }
    public FdtAccountId? FdtAccountId { get; private set; }
    public RsnReference? RsnReference { get; private set; }
    public DepositBankId? DepositBankId { get; private set; }
    public DepositFdtAccountId? DepositFdtAccountId { get; private set; }
    public DepositWalletId? DepositWalletId { get; init; }
    public Instruction? DepositInstruction { get; protected set; }
    public Instruction? PaymentInstruction { get; protected set; }

    public string? SafeTxHash { get; private set; }
    public string? RedeemTxHash { get; protected set; }

    private readonly List<SafeSignature> _signatures = [];
    public IReadOnlyList<SafeSignature> Signatures => _signatures;

    private readonly List<OrderDocument> _documents = [];
    public IReadOnlyList<OrderDocument> Documents => _documents;

    public virtual Result Signed(string safeTxHash, List<SafeSignature> signatures, UserInfo userInfo)
    {
        EntityEmptyException.ThrowIfEmpty(signatures);

        ProcessingStatus = OrderProcessingStatus.SigningInitiated;
        SafeTxHash = safeTxHash;

        foreach (var signature in signatures.Where(signature => _signatures.TrueForAll(s => s.Address != signature.Address)))
        {
            if (_signatures.Count == 0)
                AddDomainEvent(new SigningInitiatedEvent(signature, Id, ClientId, userInfo));
            else
                AddDomainEvent(new SigningOccurredEvent(signature, Id, ClientId, userInfo));

            _signatures.Add(signature);
        }

        UpgradeVersion();

        return Result.Ok();
    }

    public abstract Result Executed(SafeSignature signature, UserInfo userInfo);

    protected Result TransactionExecuted(SafeSignature signature)
    {
        if (ProcessingStatus != OrderProcessingStatus.SigningInitiated)
            return Result.Fail(new EntityInvalidOperation(nameof(Order), $"Order processing status {ProcessingStatus} doesn't permit transaction execution"));

        ProcessingStatus = OrderProcessingStatus.TransactionExecuted;
        _signatures.Add(signature);

        UpgradeVersion();

        return Result.Ok();
    }

    public virtual Result AddRsnReference(RsnReference rsnReference, UserInfo userInfo)
    {
        EntityNullException.ThrowIfNull(FdtAccountId, nameof(FdtAccount));

        RsnReference = rsnReference;
        ProcessingStatus = OrderProcessingStatus.RsnReferenceCreated;

        AddDomainEvent(new OrderRsnReferenceAddedEvent(Id, ClientId, userInfo));

        return Result.Ok();
    }

    protected void OrderInProgress()
    {
        if (Status is not OrderStatus.Initiated)
            throw new EntityInvalidStateException(nameof(Order));

        Status = OrderStatus.InProgress;
        UpgradeVersion();
    }

    protected void OrderCompleted()
    {
        if (Status is not OrderStatus.InProgress)
            throw new EntityInvalidStateException(nameof(Order));

        Status = OrderStatus.Completed;
        UpgradeVersion();
    }

    protected void OrderCanceled()
    {
        if (Status is OrderStatus.Completed)
            throw new EntityInvalidStateException(nameof(Order));

        Status = OrderStatus.Cancelled;
        UpgradeVersion();
    }

    public bool IsBankTransfer() => BankAccountId.HasValue && DepositBankId.HasValue;
}

[StronglyTypedId]
public readonly partial struct OrderId;
