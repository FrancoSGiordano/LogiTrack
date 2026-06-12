using BuildingBlocks.CQRS;
using Fleet.Application.DTOs.Trip;
using Fleet.Application.Interfaces;
using Mapster;

namespace Fleet.Application.Trips.Queries.GetTrips
{
    public class GetTripsQueryHandler
        (ITripRepository repository)
        : IQueryHandler<GetTripsQuery, ICollection<TripResponse>>
    {
        public async Task<ICollection<TripResponse>> Handle(GetTripsQuery query, CancellationToken cancellationToken)
        {
            var trips = await repository.GetTrips();

            return trips.Adapt<ICollection<TripResponse>>();
        }
    }
}
