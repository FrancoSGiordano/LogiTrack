using BuildingBlocks.CQRS;
using Fleet.Application.DTOs.Truck;

namespace Fleet.Application.Trucks.Commands.CreateTruck
{
    public record CreateTruckCommand(
        string Model,
        string LicensePlate,
        double MaxCargoCapacityKg
    ) : ICommand<TruckResponse>;
}
