namespace SmartCoinOS.Domain.Orders;

public enum OrderProcessingStatus
{
    DepositInstructionCreated = 0,
    DepositInstructionCompleted = 1,
    RsnReferenceCreated = 2,
    RsnAmountAdded = 3,

    DepositTxHashSet = 10,
    TokenDepositConfirmed = 11,
    PaymentInstructionCreated = 12,
    PaymentInstructionCompleted = 13,
    WithdrawalConfirmed = 14,

    SigningInitiated = 30,
    TransactionExecuted = 31,

    Cancelled = 32
}