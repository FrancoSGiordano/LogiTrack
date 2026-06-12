using BuildingBlocks.CQRS;
using Fleet.Application.DTOs.Trip;

namespace Fleet.Application.Trips.Commands.CreateTrip
{
    public record CreateTripCommand(
        Guid TruckId,
        string Origin,
        string Destination,
        double OriginLat,
        double OriginLon,
        double DestionationLat,
        double DestionationLon
    ) : ICommand<TripResponse>;
    
}
