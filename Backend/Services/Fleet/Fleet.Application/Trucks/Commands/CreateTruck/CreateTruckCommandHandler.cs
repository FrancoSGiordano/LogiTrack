using BuildingBlocks.CQRS;
using Fleet.Application.DTOs.Truck;
using Fleet.Application.Exceptions;
using Fleet.Application.Interfaces;
using Fleet.Domain.Entities;

namespace Fleet.Application.Trucks.Commands.CreateTruck
{
    public class CreateTruckCommandHandler
        (ITruckRepository repository)
        : ICommandHandler<CreateTruckCommand, TruckResponse>
    {
        public async Task<TruckResponse> Handle(CreateTruckCommand command, CancellationToken cancellationToken)
        {
            if(!await repository.IsLicensePlateUnique(command.LicensePlate, null))
            {
                throw new LicensePlateAlreadyExistsException(command.LicensePlate);
            }

            var newTruck = new Truck(command.Model, command.LicensePlate, command.MaxCargoCapacityKg);

            await repository.AddTruck(newTruck);

            return new TruckResponse
            {
                Id = newTruck.Id,
                Model = newTruck.Model,
                LicensePlate = newTruck.LicensePlate,
                MaxCargoCapacityKg = newTruck.MaxCargoCapacityKg,
                Status = newTruck.Status.ToString(),
                CreatedAt = newTruck.CreatedAt
            };
        }
    }
}
