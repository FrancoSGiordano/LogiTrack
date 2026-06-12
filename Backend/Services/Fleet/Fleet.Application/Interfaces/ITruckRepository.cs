using Fleet.Domain.Entities;

namespace Fleet.Application.Interfaces
{
    public interface ITruckRepository
    {
        Task<Truck?> GetTruckById(Guid Id);
        Task<IEnumerable<Truck>> GetTrucks();
        Task AddTruck(Truck Truck);
        Task UpdateTruck(Truck Truck);
        Task<bool> IsLicensePlateUnique(string Plate, Guid? currentTruckId);
    }
}
