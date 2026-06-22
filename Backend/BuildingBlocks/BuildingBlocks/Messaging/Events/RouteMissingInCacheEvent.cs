namespace BuildingBlocks.Messaging.Events
{
    public record class RouteMissingInCacheEvent(Guid? TripId, DateTime DetectedAt);
    
}
