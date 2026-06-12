using BuildingBlocks.CQRS;
using Fleet.Application.DTOs.Trip;

namespace Fleet.Application.Trips.Commands.UpdateTrip
{
    public record UpdateTripCommand
    (
        Guid Id,
        Guid TruckId,
        string Origin,
        string Destination,
        double OriginLat,
        double OriginLon,
        double DestinationLat,
        double DestinationLon
    ) : ICommand<TripResponse>;    
}
