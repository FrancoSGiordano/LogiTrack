using BuildingBlocks.CQRS;
using BuildingBlocks.Interfaces;
using BuildingBlocks.Messaging.Events;
using Fleet.Application.DTOs.Trip;
using Fleet.Application.Exceptions;
using Fleet.Application.Interfaces;
using Fleet.Domain.Entities;
using Mapster;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Fleet.Application.Trips.Commands.UpdateTripStatus
{
    public class UpdateTripStatusCommandHandler
        (ITripRepository repository, IRoutingService routingService, IDistributedCache cache, IEventBus bus)
        : ICommandHandler<UpdateTripStatusCommand, TripResponse>
    {
        public async Task<TripResponse> Handle(UpdateTripStatusCommand command, CancellationToken cancellationToken)
        {

            var trip = await repository.GetTripWithTruckByIdAsync(command.Id);

            if (trip == null)
            {
                throw new TripNotFoundException(command.Id);
            }


            switch (command.Status.ToUpper())
            {
                case "INPROGRESS":

                    if(trip.TruckId == null)
                    {
                        throw new TripHasNotTruckAssignedException(trip.Id);
                    }

                    trip.StartTrip();
                    await SaveRouteOnRedis(trip);
                    await bus.PublishAsync(new TripStartedEvent(
                        trip.Id,
                        trip.TruckId.Value,
                        trip.OriginLat,
                        trip.OriginLon,
                        trip.DestinationLat,
                        trip.DestinationLon
                        ));
                    break;
                case "COMPLETED":
                    trip.CompleteTrip(command.finishLat, command.finishLon);
                    await RemoveRouteFromRedis(trip.Id);
                    break;
                default:
                    throw new InvalidTripStatusException(command.Status);
            }

            await repository.UpdateTrip(trip);

            return trip.Adapt<TripResponse>();
        }

        private async Task SaveRouteOnRedis(Trip Trip)
        {
            var polyline = await routingService.GetRoutePolyline(
                Trip.OriginLat, Trip.OriginLon,
                Trip.DestinationLat, Trip.DestinationLon
            );

            var cacheKey = $"trip_route:{Trip.Id}";
            var polylineJson = JsonSerializer.Serialize(polyline);

            var cacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(2) };
            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(polyline), cacheOptions);
        }

        private async Task RemoveRouteFromRedis(Guid TripId)
        {
            await cache.RemoveAsync($"trip_route:{TripId}");
        }
    }
}
