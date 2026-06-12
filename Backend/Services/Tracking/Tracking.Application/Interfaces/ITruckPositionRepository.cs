using Tracking.Core.Entities;

namespace Tracking.Application.Interfaces
{
    public interface ITruckPositionRepository
    {
        Task AddTruckPosition(TruckPosition TruckPosition);
    }
}
