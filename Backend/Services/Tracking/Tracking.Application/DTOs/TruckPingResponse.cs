namespace Tracking.Application.DTOs
{
    public record TruckPingResponse
    (
        Guid TruckId,
        Guid? TripId,
        double Latitude,
        double Longitude,
        double Speed,
        bool IsDeviated,
        bool IsCompleted,
        DateTime Timestamp
    );
}
