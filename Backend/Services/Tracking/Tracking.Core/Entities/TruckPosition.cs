using Tracking.Core.Exceptions;

namespace Tracking.Core.Entities
{
    public class TruckPosition
    {
        public Guid Id { get; private set; }
        public Guid TruckId { get; private set; }
        public Guid? TripId { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public double Speed { get; private set; }
        public DateTime Timestamp { get; private set; }
        public bool IsDeviation { get; private set; }

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
