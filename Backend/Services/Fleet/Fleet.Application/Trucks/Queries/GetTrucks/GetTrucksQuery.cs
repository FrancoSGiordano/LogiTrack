using BuildingBlocks.CQRS;
using Fleet.Application.DTOs.Truck;

namespace Fleet.Application.Trucks.Queries.GetTrucks
{
    public record GetTrucksQuery() : IQuery<IEnumerable<TruckResponse>>;
}
