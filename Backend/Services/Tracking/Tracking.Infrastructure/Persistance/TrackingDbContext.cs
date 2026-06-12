using Microsoft.EntityFrameworkCore;
using Tracking.Core.Entities;
using System.Reflection;

namespace Tracking.Infrastructure.Persistance
{
    public class TrackingDbContext : DbContext
    {
        public TrackingDbContext(DbContextOptions<TrackingDbContext> options) : base(options)
        {

        }

        public DbSet<TruckPosition> TruckPositions => Set<TruckPosition>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
