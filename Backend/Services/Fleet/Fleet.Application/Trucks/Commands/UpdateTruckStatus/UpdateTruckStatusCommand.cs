using BuildingBlocks.CQRS;
using Fleet.Application.DTOs.Truck;

namespace Fleet.Application.Trucks.Commands.UpdateTruckStatus
{
    public record UpdateTruckStatusCommand
        (Guid Id, string NewStatus)
        : ICommand<TruckResponse>;
}
