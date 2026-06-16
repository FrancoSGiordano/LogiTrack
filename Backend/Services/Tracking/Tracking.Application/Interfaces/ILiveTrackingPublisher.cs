using Tracking.Core.Entities;

namespace Tracking.Application.Interfaces
{
    public interface ILiveTrackingPublisher
    {
        Task PublishPosition(TruckPosition Position);
    }
}
