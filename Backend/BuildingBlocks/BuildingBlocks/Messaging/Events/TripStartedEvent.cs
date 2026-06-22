
namespace BuildingBlocks.Messaging.Events
{
    public record TripStartedEvent(
        Guid TripId, 
        Guid TruckId, 
        double OriginLat, 
        double OriginLon,
        double DestinationLat,
        double DestinationLon
        );
   
}
