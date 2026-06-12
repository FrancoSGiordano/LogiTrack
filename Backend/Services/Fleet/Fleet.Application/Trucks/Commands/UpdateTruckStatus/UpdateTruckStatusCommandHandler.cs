using BuildingBlocks.CQRS;
using Fleet.Application.DTOs.Truck;
using Fleet.Application.Interfaces;

namespace Fleet.Application.Trucks.Commands.UpdateTruckStatus
{
    public class UpdateTruckStatusCommandHandler
        (ITruckRepository repository)
        : ICommandHandler<UpdateTruckStatusCommand, TruckResponse>
    {
        public async Task<TruckResponse> Handle(UpdateTruckStatusCommand command, CancellationToken cancellationToken)
        {
            var truck = await repository.GetTruckById(command.Id);

            if (truck == null)
            {
                throw new KeyNotFoundException($"El camión con ID {command.Id} no fue encontrado.");
            }

            switch (command.NewStatus.ToUpper())
            {
                case "ONROUTE":
                    truck.DispatchToRoute();
                    break;
                case "INBASE":
                    truck.ReturnToBase();
                    break;
                case "UNDERMAINTENANCE":
                    truck.SendToMaintenance();
                    break;
                case "OUTOFSERVICE":
                    truck.SetOutOfService();
                    break;
                default:
                    throw new ArgumentException("Estado no válido. Use: OnRoute, InBase, UnderMaintenance, OutOfService.");
            }

            await repository.UpdateTruck(truck);

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
