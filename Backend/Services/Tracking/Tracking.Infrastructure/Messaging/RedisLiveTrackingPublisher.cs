using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using Tracking.Application.DTOs;
using Tracking.Application.Interfaces;

namespace Tracking.Infrastructure.Messaging
{
    public class RedisLiveTrackingPublisher : ILiveTrackingPublisher
    {
        private readonly IConnectionMultiplexer redis;
        private readonly ILogger<RedisLiveTrackingPublisher> logger;

        public RedisLiveTrackingPublisher(IConnectionMultiplexer redis, ILogger<RedisLiveTrackingPublisher> logger)
        {
            this.logger = logger;
            this.redis = redis;
        }

        public async Task PublishPosition(TruckPingResponse ping)
        {
            try
            {
                var suscriber = redis.GetSubscriber();
                var messageJson = JsonSerializer.Serialize(ping);

                await suscriber.PublishAsync(RedisChannel.Literal("live_tracking"), messageJson);
            }
            catch (Exception ex)
            {          
                logger.LogError(ex, "Falló la publicación en Redis Pub/Sub para el viaje {TripId}", ping.TripId);
            }
        }
    }
}
