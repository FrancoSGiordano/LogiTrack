using BuildingBlocks.CQRS;
using Fleet.Application.DTOs.Trip;
using Fleet.Application.Exceptions;
using Fleet.Application.Interfaces;
using Mapster;

namespace Fleet.Application.Trips.Commands.UpdateTrip
{
    public class UpdateTripCommandHandler
        (ITripRepository repository)
        : ICommandHandler<UpdateTripCommand, TripResponse>
    {
        public async Task<TripResponse> Handle(UpdateTripCommand command, CancellationToken cancellationToken)
        {
            var trip = await repository.GetTripById(command.Id);

            if(trip == null) 
            {
                throw new TripNotFoundException(command.Id);
            }

            trip.TruckId = command.TruckId;
            trip.Origin = command.Origin;
            trip.Destination = command.Destination;
            trip.OriginLat = command.OriginLat;
            trip.OriginLon = command.OriginLon;
            trip.DestinationLat = command.DestinationLat;
            trip.DestinationLon = command.DestinationLon;

            await repository.UpdateTrip(trip);

            return trip.Adapt<TripResponse>();
        }
    }
}
