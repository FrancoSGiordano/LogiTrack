using BuildingBlocks.CQRS;
using Fleet.Application.DTOs.Truck;

namespace Fleet.Application.Trucks.Queries.GetTruckById
{
    public record GetTruckByIdQuery(Guid Id) : IQuery<TruckResponse>;
}
