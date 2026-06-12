using BuildingBlocks.CQRS;
using Fleet.Application.DTOs.Trip;

namespace Fleet.Application.Trips.Queries.GetTripById
{
    public record GetTripByIdQuery(Guid Id) : IQuery<TripResponse>; 
}
