namespace SmartCoinOS.Domain.Results;

public sealed class EntityInvalidOperation : Error
{
    public string EntityType { get; }

    public EntityInvalidOperation(string entityType, params string[] messages)
    {
        EntityType = entityType;
        Reasons.AddRange(messages.Select(x => new Error(x)));
    }
}