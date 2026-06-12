using BuildingBlocks.CQRS;
using Fleet.Application.DTOs.Truck;

namespace Fleet.Application.Trucks.Commands.UpdateTruck
{
    public record UpdateTruckCommand
        (Guid Id, string Model, string LicensePlate, double MaxCargoCapacityKg)
        : ICommand<TruckResponse>;
}
