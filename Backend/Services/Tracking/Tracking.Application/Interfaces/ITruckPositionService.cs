using Tracking.Application.DTOs;

namespace Tracking.Application.Interfaces
{
    public interface ITruckPositionService
    {
        Task AddTruckPosition(TruckPing TruckPing);
    }
}
