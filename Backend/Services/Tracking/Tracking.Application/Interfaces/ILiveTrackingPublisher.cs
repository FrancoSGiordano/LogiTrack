using Tracking.Application.DTOs;

namespace Tracking.Application.Interfaces
{
    public interface ILiveTrackingPublisher
    {
        Task PublishPosition(TruckPingResponse ping);
    }
}
