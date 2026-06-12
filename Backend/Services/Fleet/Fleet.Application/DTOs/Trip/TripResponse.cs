namespace Fleet.Application.DTOs.Trip
{
    public record TripResponse
    {
        public Guid Id { get; init; }
        public Guid TruckId { get; init; }
        public string Origin { get; init; }
        public string Destination { get; init; }
        public double OriginLat { get; init; }
        public double OriginLon { get; init; }
        public double DestinationLat { get; init; }
        public double DestinationLon { get; init; }
        public string Status { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime StartedAt { get; init; }
        public DateTime CompletedAt { get; init; }
    }
}
