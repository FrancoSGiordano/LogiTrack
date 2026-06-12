using BuildingBlocks.CQRS;
using Fleet.Application.DTOs.Truck;
using Fleet.Application.Exceptions;
using Fleet.Application.Interfaces;

namespace Fleet.Application.Trucks.Commands.UpdateTruck
{
    public class UpdateTruckCommandHanlder
        (ITruckRepository repository)
        : ICommandHandler<UpdateTruckCommand, TruckResponse>
    {
        public async Task<TruckResponse> Handle(UpdateTruckCommand command, CancellationToken cancellationToken)
        {
            var truck = await repository.GetTruckById(command.Id);

            if (truck == null)
            {
                throw new TruckNotFoundException(command.Id);
            }

            if(!await repository.IsLicensePlateUnique(command.LicensePlate, command.Id))
            {
                throw new LicensePlateAlreadyExistsException(command.LicensePlate);
            }

            truck.UpdateDetails(command.Model, command.LicensePlate, command.MaxCargoCapacityKg);

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
