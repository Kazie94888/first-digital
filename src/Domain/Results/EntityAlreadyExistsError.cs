namespace SmartCoinOS.Domain.Results;

public sealed class EntityAlreadyExistsError : Error
{
    public EntityAlreadyExistsError(string entityType, params string[] messages)
    {
        EntityType = entityType;
        Reasons.AddRange(messages.Select(x => new Error(x)));
    }

    public string EntityType { get; }
}
