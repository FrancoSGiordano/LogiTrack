using BuildingBlocks.CQRS;
using Fleet.Application.DTOs.Trip;
using Fleet.Application.Interfaces;
using Fleet.Domain.Entities;
using Fleet.Domain.Enums;
using Mapster;

namespace Fleet.Application.Trips.Commands.CreateTrip
{
    public class CreateTripCommandHandler
        (ITripRepository repository)
        : ICommandHandler<CreateTripCommand, TripResponse>
    {
        public async Task<TripResponse> Handle(CreateTripCommand command, CancellationToken cancellationToken)
        {
            var newTrip = new Trip
            {
                Id = Guid.NewGuid(),
                TruckId = command.TruckId,
                Origin = command.Origin.ToUpper(),
                Destination = command.Destination.ToUpper(),
                OriginLat = command.OriginLat,
                OriginLon = command.OriginLon,
                DestinationLat = command.DestionationLat,
                DestinationLon = command.DestionationLon,
                Status = TripStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await repository.CreateTrip(newTrip);

            return newTrip.Adapt<TripResponse>();
        }
    }
}
