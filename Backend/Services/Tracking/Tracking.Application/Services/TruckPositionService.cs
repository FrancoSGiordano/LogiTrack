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

        public TruckPositionService(ITruckPositionRepository repository, ILiveTrackingPublisher publisher, ITripRouteCache cache)
        {
            this.repository = repository;
            this.publisher = publisher;
            this.cache = cache;
        }

        public async Task AddTruckPosition(TruckPing TruckPing)
        {
            var position = TruckPing.Adapt<TruckPosition>();

            position.Timestamp = position.Timestamp.ToUniversalTime();

            var lastPing = await cache.GetLastPing(TruckPing.TripId);

            if(lastPing != null)
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

            await cache.SetLastPing(TruckPing.TripId, TruckPing);

            var routeCoordinates = await cache.GetRoute(TruckPing.TripId);

            if(routeCoordinates != null && routeCoordinates.Any())
            {
                position.IsDeviation = GeoCalculator.IsDeviated(TruckPing.Latitude, TruckPing.Longitude, routeCoordinates, 100);
            }

            await repository.AddTruckPosition(position);

            await publisher.PublishPosition(position);
        }  
    }
}
