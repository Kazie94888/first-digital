using MediatR;

namespace SmartCoinOS.Domain.SeedWork;

public abstract class AggregateRoot : Entity
{
    public Guid CurrentVersion { get; private set; }
    private readonly List<INotification> _domainEvents = [];

    public IReadOnlyList<INotification> DomainEvents => _domainEvents.AsReadOnly();

    internal void AddDomainEvent(INotification eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem)
    {
        _domainEvents.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Invoke version upgrade every time there's a risk of race condition.
    /// 
    /// We utilize the <see cref="CurrentVersion"/> property to track the aggregate version. 
    /// By versioning the aggregate, we can use optimistic concurrency features to guarantee the aggregates state.
    /// </summary>
    public void UpgradeVersion()
    {
        CurrentVersion = Guid.NewGuid();
    }
}