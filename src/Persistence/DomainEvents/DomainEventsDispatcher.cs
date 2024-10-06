using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartCoinOS.Domain.SeedWork;

namespace SmartCoinOS.Persistence.DomainEvents;

public sealed class DomainEventsDispatcher : IDomainEventsDispatcher
{
    private readonly IMediator _mediator;
    private readonly ILogger<DomainEventsDispatcher> _logger;

    public DomainEventsDispatcher(IMediator mediator, ILogger<DomainEventsDispatcher> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<int> DispatchDomainEventsAsync(DataContext context, CancellationToken cancellationToken)
    {
        var entities = context.ChangeTracker.Entries<AggregateRoot>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();
        var events = entities.SelectMany(x => x.DomainEvents).ToList();
        entities.ForEach(x => x.ClearDomainEvents());

        foreach (var @event in events)
        {
            var domainEventName = @event.GetType().Name;
            var eventData = JsonSerializer.Serialize(@event);

            await _mediator.Publish(@event, cancellationToken);
            _logger.LogDebug("Published domain event {EventName}. EventData - {EventData}", domainEventName, eventData);
        }

        return events.Count;
    }
}
