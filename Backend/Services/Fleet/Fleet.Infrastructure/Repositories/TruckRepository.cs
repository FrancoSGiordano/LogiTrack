using Fleet.Application.Interfaces;
using Fleet.Domain.Entities;
using Fleet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fleet.Infrastructure.Repositories
{
    public class TruckRepository : ITruckRepository
    {
        private readonly FleetDbContext _context;

        public TruckRepository(FleetDbContext context)
        {
            _context = context;
        }

        public async Task<Truck?> GetTruckById(Guid Id)
        {
            return await _context.Trucks.FindAsync(Id);
        }

        public async Task<IEnumerable<Truck>> GetTrucks()
        {
            return await _context.Trucks.ToListAsync();
        }

        public async Task AddTruck(Truck Truck)
        {
            _context.Trucks.Add(Truck);
            await _context.SaveChangesAsync();
        }    

        public async Task UpdateTruck(Truck Truck)
        {
            _context.Trucks.Update(Truck);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsLicensePlateUnique(string Plate, Guid? currentTruckId)
        {
            var upperPlate = Plate.ToUpper();

            var query = _context.Trucks.Where(t => t.LicensePlate.ToUpper() == upperPlate);

            if (currentTruckId.HasValue)
            {
                query = query.Where(t => t.Id != currentTruckId);
            }
            
            return !await query.AnyAsync();
        }
    }
}
