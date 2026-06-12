using BuildingBlocks.CQRS;
using Fleet.Application.DTOs.Trip;

namespace Fleet.Application.Trips.Queries.GetTrips
{
    public record GetTripsQuery() : IQuery<ICollection<TripResponse>>;  
}
