using Fleet.Domain.Enums;

namespace Fleet.Application.DTOs.Truck
{
    public record TruckResponse
    {
        public Guid Id { get; init; }
        public string Model { get; init; }
        public string LicensePlate { get; init; }
        public double MaxCargoCapacityKg { get; init; }
        public string Status { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
