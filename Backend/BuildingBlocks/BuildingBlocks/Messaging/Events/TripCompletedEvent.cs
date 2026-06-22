namespace BuildingBlocks.Messaging.Events
{
    public record TripCompletedEvent(Guid TripId, double FinishLat, double FinishLon);
    
}
