using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Persistence.DomainEvents;

namespace SmartCoinOS.Backoffice.Api.Application.Behaviors;

internal sealed class TransactionCommandBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
    where TResponse : ResultBase, new()
{
    private readonly DataContext _context;
    private readonly IDomainEventsDispatcher _domainEventsDispatcher;

    public TransactionCommandBehavior(DataContext context, IDomainEventsDispatcher domainEventsDispatcher)
    {
        _context = context;
        _domainEventsDispatcher = domainEventsDispatcher;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var result = await next();

            if (!result.IsSuccess)
                return result;

            await _context.SaveChangesAsync(cancellationToken);

            var maxAllowedEventRounds = 5;
            var eventsDispatched = await _domainEventsDispatcher.DispatchDomainEventsAsync(_context, cancellationToken);
            while (eventsDispatched > 0 && maxAllowedEventRounds > 0)
            {
                maxAllowedEventRounds -= 1;
                await _context.SaveChangesAsync(cancellationToken);
                eventsDispatched = await _domainEventsDispatcher.DispatchDomainEventsAsync(_context, cancellationToken);
            }

            if (_context.Database.CurrentTransaction != null)
                await transaction.CommitAsync(cancellationToken);

            return result;
        });
    }
}
