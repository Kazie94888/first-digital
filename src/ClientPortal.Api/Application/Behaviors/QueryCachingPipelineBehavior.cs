using FluentResults;
using MediatR;
using SmartCoinOS.Application.Abstractions.Caching;
using SmartCoinOS.Application.Abstractions.Messaging;

namespace SmartCoinOS.ClientPortal.Api.Application.Behaviors;

internal sealed class QueryCachingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>>
    where TRequest : ICachedQuery
{
    private readonly ICacheService _cacheService;

    public QueryCachingPipelineBehavior(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken cancellationToken)
    {
        return await _cacheService.GetOrCreateAsync(
            request.CacheKey,
            _ => next(),
            request.Expiration,
            useSlidingExpiration: true,
            cancellationToken);
    }
}