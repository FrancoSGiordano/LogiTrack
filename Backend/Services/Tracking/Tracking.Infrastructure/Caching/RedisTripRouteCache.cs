using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Tracking.Application.DTOs;
using Tracking.Application.Interfaces;
using Tracking.Core.Utils;

namespace Tracking.Infrastructure.Caching
{
    public class RedisTripRouteCache : ITripRouteCache
    {
        private readonly IDistributedCache cache;

        public RedisTripRouteCache(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public async Task<TruckPing> GetLastPing(Guid? TripId)
        {
            var cacheKey = $"last_ping:{TripId}";
            var json = await cache.GetStringAsync(cacheKey);

            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            return JsonSerializer.Deserialize<TruckPing>(json);
        }

        public async Task<List<Coordinate>> GetRoute(Guid? TripId)
        {
            var cacheKey = $"trip_route:{TripId}";
            var json = await cache.GetStringAsync(cacheKey);

            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            return JsonSerializer.Deserialize<List<Coordinate>>(json);
        }

        public async Task SetLastPing(Guid? TripId, TruckPing Ping)
        {
            var cacheKey = $"last_ping:{TripId}";
            var json = JsonSerializer.Serialize(Ping);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            };

            await cache.SetStringAsync(cacheKey, json, options);
        }
    }
}
