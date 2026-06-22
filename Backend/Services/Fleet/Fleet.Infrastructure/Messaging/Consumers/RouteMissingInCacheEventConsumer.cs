using BuildingBlocks.Messaging.Events;
using Fleet.Application.Interfaces;
using Fleet.Application.Trips.Queries.GetTripById;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Fleet.Infrastructure.Messaging.Consumers
{
    public class RouteMissingInCacheEventConsumer : IConsumer<RouteMissingInCacheEvent>
    {
        private readonly IDistributedCache cache;
        private readonly IRoutingService routingService;
        private readonly IMediator mediator;

        public RouteMissingInCacheEventConsumer(IDistributedCache cache, IRoutingService routingService, IMediator mediator)
        {
            this.cache = cache;
            this.routingService = routingService;
            this.mediator = mediator;
        }

        public async Task Consume(ConsumeContext<RouteMissingInCacheEvent> context)
        {
            var tripId = context.Message.TripId;
            string cacheKey = $"trip_route:{tripId}";

            var existingRoute = await cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(existingRoute))
            {
                return;
            }

            var trip = await mediator.Send(new GetTripByIdQuery(tripId?? Guid.Empty));

            if (trip == null) return;

            var route = await routingService.GetRoutePolyline(
                trip.OriginLat, trip.OriginLon,
                trip.DestinationLat, trip.DestinationLon);

            if(route != null)
            {
                await cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(route),
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) });
            }

        }
    }
}

