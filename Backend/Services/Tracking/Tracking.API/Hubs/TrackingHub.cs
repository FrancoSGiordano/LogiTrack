using Microsoft.AspNetCore.SignalR;

namespace Tracking.API.Hubs
{
    public class TrackingHub : Hub
    {
        public async Task JoinTripGroup(string tripId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"trip_{tripId}");
        }

        public async Task LeaveTripGroup(string tripId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"trip_{tripId}");

        }

    }
}
