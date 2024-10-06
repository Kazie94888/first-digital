namespace SmartCoinOS.Domain.SeedWork;

public abstract class Entity
{
    protected Entity()
    {
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public DateTimeOffset CreatedAt { get; internal set; }
    public required UserInfo CreatedBy { get; init; }
}