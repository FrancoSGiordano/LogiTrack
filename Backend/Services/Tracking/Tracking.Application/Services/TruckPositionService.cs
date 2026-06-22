using BuildingBlocks.Interfaces;
using BuildingBlocks.Messaging.Events;
using Mapster;
using Tracking.Application.DTOs;
using Tracking.Application.Interfaces;
using Tracking.Core.Entities;
using Tracking.Core.Utils;

namespace Tracking.Application.Services
{
    public class TruckPositionService : ITruckPositionService
    {
        private readonly ITruckPositionRepository repository;
        private readonly ILiveTrackingPublisher publisher;
        private readonly ITripRouteCache cache;
        private readonly IEventBus bus;

        public TruckPositionService(ITruckPositionRepository repository, ILiveTrackingPublisher publisher, ITripRouteCache cache, IEventBus bus)
        {
            this.repository = repository;
            this.publisher = publisher;
            this.cache = cache;
            this.bus = bus;
        }

        public async Task AddTruckPosition(TruckPingReceived TruckPing)
        {

            var routeCoordinates = await cache.GetRoute(TruckPing.TripId);

            if (routeCoordinates == null)
            {
                await bus.PublishAsync(new RouteMissingInCacheEvent(TruckPing.TripId, DateTime.UtcNow));

                return;
            }

            var position = TruckPing.Adapt<TruckPosition>();

            position.Timestamp = position.Timestamp.ToUniversalTime();

            var lastPing = await cache.GetLastPing(TruckPing.TripId);

            if (lastPing != null)
            {
                position.Speed = GeoCalculator.CalculateSpeedKmH(
                    lastPing.Latitude, lastPing.Longitude, lastPing.Timestamp,
                    TruckPing.Latitude, TruckPing.Longitude, TruckPing.Timestamp
                    );
            }
            else
            {
                position.Speed = 0;
            }

            var destination = routeCoordinates.LastOrDefault();

            bool isCompleted = GeoCalculator.HasArrived(TruckPing.Latitude, TruckPing.Longitude, destination.Lat, destination.Lon);

            if (isCompleted)
            {
                Console.WriteLine("Completamos el viaje !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                await bus.PublishAsync(new TripCompletedEvent(TruckPing.TripId ?? Guid.Empty, TruckPing.Latitude, TruckPing.Longitude));
            }
            else
            {
                position.IsDeviation = GeoCalculator.IsDeviated(TruckPing.Latitude, TruckPing.Longitude, routeCoordinates, 100);
            }   

            var pingResponse = new TruckPingResponse(
                position.TruckId,
                position.TripId,
                position.Latitude,
                position.Longitude,
                position.Speed,
                position.IsDeviation,
                isCompleted,
                position.Timestamp
            );

            await cache.SetLastPing(TruckPing.TripId, pingResponse);

            await repository.AddTruckPosition(position);

            await publisher.PublishPosition(pingResponse);
        }  
    }
}
