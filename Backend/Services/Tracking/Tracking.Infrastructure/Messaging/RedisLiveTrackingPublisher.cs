using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using Tracking.Application.Interfaces;
using Tracking.Core.Entities;

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

        public async Task PublishPosition(TruckPosition Position)
        {
            try
            {
                var suscriber = redis.GetSubscriber();
                var messageJson = JsonSerializer.Serialize(Position);

                await suscriber.PublishAsync(RedisChannel.Literal("live_tracking"), messageJson);
            }
            catch (Exception ex)
            {          
                logger.LogError(ex, "Falló la publicación en Redis Pub/Sub para el viaje {TripId}", Position.TripId);
            }
        }
    }
}
