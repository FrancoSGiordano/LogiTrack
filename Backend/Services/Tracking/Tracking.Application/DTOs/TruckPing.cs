namespace Tracking.Application.DTOs
{
    public record TruckPing(
        Guid TruckId,
        Guid? TripId,
        double Latitude,
        double Longitude,
        double Speed,
        DateTime Timestamp
    );

}
