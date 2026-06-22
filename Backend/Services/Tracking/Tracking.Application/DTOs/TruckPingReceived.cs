namespace Tracking.Application.DTOs
{
    public record TruckPingReceived(
        Guid TruckId,
        Guid? TripId,
        double Latitude,
        double Longitude,
        DateTime Timestamp
    );

}
