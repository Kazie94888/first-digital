namespace SmartCoinOS.Domain.Results;

public sealed class EntityInUseError : Error
{
    public EntityInUseError(string entityType, params string[] messages)
    {
        EntityType = entityType;
        Reasons.AddRange(messages.Select(x => new Error(x)));
    }

    public string EntityType { get; }
}
