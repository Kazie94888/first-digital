using FluentResults;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SmartCoinOS.Application.Abstractions.Caching;

namespace SmartCoinOS.Infrastructure.Caching;

internal sealed class CacheService : ICacheService
{
    private static readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(5);

    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<CacheService> _logger;

    public CacheService(IMemoryCache memoryCache, ILogger<CacheService> logger)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger;
    }

    public async Task<Result<T>> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<Result<T>>> factory,
        TimeSpan? expiration = null,
        bool useSlidingExpiration = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Cache key cannot be null or empty.", nameof(key));

        var value = await _memoryCache.GetOrCreateAsync(key, async entry =>
        {
            if (useSlidingExpiration)
                entry.SetSlidingExpiration(expiration ?? _defaultExpiration);
            else
                entry.SetAbsoluteExpiration(expiration ?? _defaultExpiration);

            var result = await factory(cancellationToken);

            // Avoid caching null or default values
            if (result.IsFailed)
            {
                _logger.LogWarning("Factory method returned a default value for key {Key}. Value not cached", key);
                return default;
            }

            return result.Value;
        });
        if (EqualityComparer<T>.Default.Equals(value, default))
            return Result.Fail<T>("Value not found");
        return Result.Ok(value!);
    }
}
