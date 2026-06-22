namespace Fleet.Application.DTOs.Trip
{
    public record TripRequest
    {
        public Guid? TruckId { get; init; }
        public string Origin { get; init; }
        public string Destination { get; init; }
        public double OriginLat { get; init; }
        public double OriginLon { get; init; }
        public double DestinationLat { get; init; }
        public double DestinationLon { get; init; }
    }
}
