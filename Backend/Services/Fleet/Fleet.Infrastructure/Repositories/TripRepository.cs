
using Fleet.Application.Interfaces;
using Fleet.Domain.Entities;
using Fleet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fleet.Infrastructure.Repositories
{
    public class TripRepository : ITripRepository
    {
        private readonly FleetDbContext context;

        public TripRepository(FleetDbContext context)
        {
            this.context = context;
        }

        public async Task CreateTrip(Trip Trip)
        {
            context.Trips.Add(Trip);
            await context.SaveChangesAsync();
        }

        public async Task UpdateTrip(Trip Trip)
        {
            context.Trips.Update(Trip);
            await context.SaveChangesAsync();
        }

        public async Task<Trip?> GetTripById(Guid Id)
        {
            return await context.Trips.FindAsync(Id);
        }

        public async Task<Trip?> GetTripWithTruckByIdAsync(Guid Id)
        {
            return await context.Trips
                .Include(t => t.Truck) 
                .FirstOrDefaultAsync(t => t.Id == Id);
        }

        public async Task<ICollection<Trip>> GetTrips()
        {
            return await context.Trips.ToListAsync();
        }    
    }
}
