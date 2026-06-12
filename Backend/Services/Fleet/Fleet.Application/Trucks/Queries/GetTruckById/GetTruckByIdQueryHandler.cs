using BuildingBlocks.CQRS;
using Fleet.Application.DTOs.Truck;
using Fleet.Application.Exceptions;
using Fleet.Application.Interfaces;

namespace Fleet.Application.Trucks.Queries.GetTruckById
{
    public class GetTruckByIdQueryHandler
        (ITruckRepository repository)
        : IQueryHandler<GetTruckByIdQuery, TruckResponse>
    {
        public async Task<TruckResponse> Handle(GetTruckByIdQuery query, CancellationToken cancellationToken)
        {
            var truck = await repository.GetTruckById(query.Id);

            if (truck == null)
            {
                throw new TruckNotFoundException(query.Id);
            }

            return new TruckResponse
            {
                Id = truck.Id,
                Model = truck.Model,
                LicensePlate = truck.LicensePlate,
                MaxCargoCapacityKg = truck.MaxCargoCapacityKg,
                Status = truck.Status.ToString(),
                CreatedAt = truck.CreatedAt,
            };
        }
    }
}
