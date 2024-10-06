namespace SmartCoinOS.Backoffice.Api.Base.Dto;

public sealed record EntityId
{
    public EntityId(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}