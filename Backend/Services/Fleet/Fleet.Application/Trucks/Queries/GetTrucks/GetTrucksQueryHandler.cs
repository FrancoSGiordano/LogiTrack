using BuildingBlocks.CQRS;
using Fleet.Application.DTOs.Truck;
using Fleet.Application.Interfaces;

namespace Fleet.Application.Trucks.Queries.GetTrucks
{
    public class GetTrucksQueryHandler
        (ITruckRepository repository)
        : IQueryHandler<GetTrucksQuery, IEnumerable<TruckResponse>>
    {
        public async Task<IEnumerable<TruckResponse>> Handle(GetTrucksQuery query, CancellationToken cancellationToken)
        {
            var trucks = await repository.GetTrucks();

            return trucks.Select(truck => new TruckResponse
                {
                    Id = truck.Id,
                    Model = truck.Model,
                    LicensePlate = truck.LicensePlate,
                    MaxCargoCapacityKg = truck.MaxCargoCapacityKg,
                    Status = truck.Status.ToString(),
                    CreatedAt = truck.CreatedAt,
                } 
            );
        }
    }

}
