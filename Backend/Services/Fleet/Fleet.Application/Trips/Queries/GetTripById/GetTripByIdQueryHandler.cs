using BuildingBlocks.CQRS;
using Fleet.Application.DTOs.Trip;
using Fleet.Application.Exceptions;
using Fleet.Application.Interfaces;
using Mapster;

namespace Fleet.Application.Trips.Queries.GetTripById
{
    public class GetTripByIdQueryHandler
        (ITripRepository repository)
        : IQueryHandler<GetTripByIdQuery, TripResponse>
    {
        public async Task<TripResponse> Handle(GetTripByIdQuery query, CancellationToken cancellationToken)
        {
            var trip = await repository.GetTripById(query.Id);

            if (trip == null)
            {
                throw new TripNotFoundException(query.Id);
            }

            return trip.Adapt<TripResponse>();
        }
    }
}
