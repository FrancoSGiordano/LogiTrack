using Fleet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Fleet.Infrastructure.Persistence
{
    public class FleetDbContext : DbContext
    {
        public FleetDbContext(DbContextOptions<FleetDbContext> options) : base(options) { }

        public DbSet<Truck> Trucks => Set<Truck>();

        public DbSet<Trip> Trips => Set<Trip>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
