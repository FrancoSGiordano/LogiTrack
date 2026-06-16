using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using System.Text.Json;
using Tracking.API.Hubs;

namespace Tracking.API
{
    public class RedisSubscribeWorker : BackgroundService
    {
        private readonly IConnectionMultiplexer redis;
        private readonly IHubContext<TrackingHub> hub;
        private readonly ILogger<RedisSubscribeWorker> logger;

        public RedisSubscribeWorker(IConnectionMultiplexer redis, IHubContext<TrackingHub> hub, ILogger<RedisSubscribeWorker> logger)
        {
            this.redis = redis;
            this.hub = hub;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var suscriber = redis.GetSubscriber();

            await suscriber.SubscribeAsync(RedisChannel.Literal("live_tracking"), async (channel, message) =>
            {
                try
                {
                    var jsonString = message.ToString();

                    using var document = JsonDocument.Parse(jsonString);
                    var root = document.RootElement;

                    if (root.TryGetProperty("TripId", out var tripIdProperty) && tripIdProperty.GetGuid() != Guid.Empty)
                    {
                        string tripId = tripIdProperty.GetGuid().ToString();
                        await hub.Clients.Group($"trip_{tripId}").SendAsync("ReceiveTruckPosition", jsonString, stoppingToken);
                    }

                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error al retransmitir mensaje desde Redis hacia SignalR");
                }
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
