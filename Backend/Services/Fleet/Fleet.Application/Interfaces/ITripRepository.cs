using Fleet.Domain.Entities;

namespace Fleet.Application.Interfaces
{
    public interface ITripRepository
    {
        Task CreateTrip(Trip Trip);
        Task UpdateTrip(Trip Trip);
        Task<ICollection<Trip>> GetTrips();
        Task<Trip?> GetTripById(Guid Id);
        Task<Trip?> GetTripWithTruckByIdAsync(Guid Id);
    }
}
