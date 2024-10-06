namespace SmartCoinOS.Domain.Results;

public sealed class AggregateChangedError : Error
{
    public AggregateChangedError(string message)
    {
        Reasons.Add(new Error(message));
    }
}