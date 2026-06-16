using Tracking.Application.DTOs;
using Tracking.Core.Utils;

namespace Tracking.Application.Interfaces
{
    public interface ITripRouteCache
    {
        Task<List<Coordinate>> GetRoute(Guid? TripId);
        Task<TruckPing> GetLastPing(Guid? TripId);
        Task SetLastPing(Guid? TripId, TruckPing Ping);
    }
}
