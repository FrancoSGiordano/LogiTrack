using Tracking.Core.Exceptions;

namespace Tracking.Core.Entities
{
    public class TruckPosition
    {
        public Guid Id { get; set; }
        public Guid TruckId { get; set; }
        public Guid? TripId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsDeviation { get; set; }

        public TruckPosition(Guid truckId, double latitude, double longitude, double speed, DateTime timestamp, Guid? tripId = null)
        {
            ValidateCoordinates(latitude, longitude);

            Id = Guid.NewGuid();
            TruckId = truckId;
            TripId = tripId;
            Latitude = latitude;
            Longitude = longitude;
            Speed = speed;
            Timestamp = timestamp;
            IsDeviation = false; 
        }

        public void MarkAsDeviation()
        {
            IsDeviation = true;
        }

        private void ValidateCoordinates(double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
            {
                throw new InvalidGeographicCoordinatesException(latitude, longitude);
            }
        }
    }
}
