using Fleet.Domain.Enums;
using Fleet.Domain.Exceptions;

namespace Fleet.Domain.Entities
{
    public class Trip
    {
        public Guid Id { get; set; }
        public Guid TruckId { get; set; }
        public Truck Truck { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public double OriginLat { get; set; }
        public double OriginLon { get; set; }
        public double DestinationLat { get; set; }
        public double DestinationLon { get; set; }
        public TripStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public void StartTrip()
        {
            if(Status == TripStatus.Compeleted)
            {
                throw new TripAlreadyCompletedException(Id, TruckId);
            }

            Status = TripStatus.InProgress;
            StartedAt = DateTime.UtcNow;
            Truck.DispatchToRoute();
        }

        public void CompleteTrip()
        {
            Status = TripStatus.Compeleted;
            CompletedAt = DateTime.UtcNow;
            Truck.SetAvailable();
        }
    }
}
