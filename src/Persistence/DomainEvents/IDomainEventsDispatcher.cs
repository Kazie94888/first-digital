namespace SmartCoinOS.Persistence.DomainEvents;

public interface IDomainEventsDispatcher
{
    Task<int> DispatchDomainEventsAsync(DataContext context, CancellationToken cancellationToken);
}
