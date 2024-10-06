namespace SmartCoinOS.Domain.Results;

public sealed class EntityAlreadyArchivedError : Error
{
    public EntityAlreadyArchivedError(string entityType, params string[] messages)
    {
        EntityType = entityType;
        Reasons.AddRange(messages.Select(x => new Error(x)));
    }

    public string EntityType { get; }
}