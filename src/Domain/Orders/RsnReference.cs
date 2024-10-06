namespace SmartCoinOS.Domain.Orders;
public sealed record RsnReference
{
    public string TransactionId { get; init; }

    public RsnReference(string transactionId)
    {
        TransactionId = transactionId;
    }
}
