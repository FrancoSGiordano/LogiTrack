using Tracking.Application.Interfaces;
using Tracking.Core.Entities;
using Tracking.Infrastructure.Persistance;

namespace Tracking.Infrastructure.Repositories
{
    public class TruckPositionRepository : ITruckPositionRepository
    {
        private readonly TrackingDbContext context;

        public TruckPositionRepository(TrackingDbContext context)
        {
            this.context = context;
        }

        public async Task AddTruckPosition(TruckPosition TruckPosition)
        {
            context.TruckPositions.Add(TruckPosition);
            await context.SaveChangesAsync();
        }
    }
}
