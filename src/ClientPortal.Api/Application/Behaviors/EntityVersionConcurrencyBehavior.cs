using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartCoinOS.Application.Abstractions.Messaging;
using SmartCoinOS.Domain.Results;

namespace SmartCoinOS.ClientPortal.Api.Application.Behaviors;

internal sealed class EntityVersionConcurrencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
    where TResponse : ResultBase, new()
{
    readonly ILogger<EntityVersionConcurrencyBehavior<TRequest, TResponse>> _logger;
    public EntityVersionConcurrencyBehavior(ILogger<EntityVersionConcurrencyBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (DbUpdateConcurrencyException ce)
        {
            string? entityName = string.Empty;
            object? entityId = string.Empty;

            if (ce.Entries.Count > 0)
            {
                var firstEntry = ce.Entries[0];
                var runtimeValues = firstEntry.CurrentValues;

                var idProp = runtimeValues.Properties.FirstOrDefault(x => x.Name == "Id");
                if (idProp is not null)
                    entityId = runtimeValues[idProp];

                entityName = firstEntry.Entity.ToString();
            }

            _logger.LogError(ce, "Entity '{entityName}' with identifier '{entityId}' collided in a race condition.", entityName, entityId);

            var result = new TResponse();
            result.Reasons.Add(new AggregateChangedError("This entity has changed since the last time you visited it, and an automated merge was not possible. Please reload the data and try again."));
            return result;
        }
    }
}
