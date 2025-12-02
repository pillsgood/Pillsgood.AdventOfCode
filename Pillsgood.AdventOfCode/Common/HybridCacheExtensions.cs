using System.Diagnostics;
using Microsoft.Extensions.Caching.Hybrid;

namespace Pillsgood.AdventOfCode.Common;

internal static class HybridCacheExtensions
{
    extension(HybridCache cache)
    {
        public async ValueTask<T?> GetAsync<T>(string key, HybridCacheEntryOptions? options = null, CancellationToken cancellationToken = new())
            where T : notnull
        {
            options = new HybridCacheEntryOptions
            {
                Flags = options?.Flags ?? HybridCacheEntryFlags.None | HybridCacheEntryFlags.DisableUnderlyingData,
            };

            return await cache.GetOrCreateAsync(
                key,
                _ => ValueTask.FromException<T?>(new UnreachableException()),
                options,
                null,
                cancellationToken);
        }
    }
}