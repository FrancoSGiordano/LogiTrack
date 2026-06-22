namespace Tracking.Core.Utils
{
    public record Coordinate(double Lat, double Lon);
    public static class GeoCalculator
    {
        private const double EarthRadiumKm = 6371.0;

        public static double CalculateSpeedKmH(
            double lat1, double lon1, DateTime time1,
            double lat2, double lon2, DateTime time2)
        {
            double distanceKm = CalculateHaversineDistanceKm(lat1, lon1, lat2, lon2);
            double timeDifferenceHours = (time2 - time1).TotalHours;

            if (timeDifferenceHours <= 0) return 0;

            double speed = distanceKm / timeDifferenceHours;

            return speed > 200 ? 0 : Math.Round(speed, 2);
        }

        public static bool IsDeviated(double currentLat, double currentLon, List<Coordinate> route, double toleranceMeters)
        {
            if (route == null || route.Count < 2) 
            {
                Console.WriteLine("❌ [GEO-CALCULATOR] ABORTANDO: La ruta tiene menos de 2 puntos. Retornando FALSE.");
                return false;
            } 

            double minDistanceMeters = double.MaxValue;

            for(int i = 0; i < route.Count - 1; i++) 
            {
                double distance = DistanceToSegmentMeters(
                    currentLat, currentLon,
                    route[i].Lat, route[i].Lon,
                    route[i + 1].Lat, route[i + 1].Lon
                    );

                if (distance <= toleranceMeters)
                {
                    Console.WriteLine($"[BINGO] Cortó el bucle. Distancia calculada: {distance}m");
                    Console.WriteLine($"[DATOS] Ping: ({currentLat}, {currentLon}) | Segmento: ({route[i].Lat}, {route[i].Lon}) a ({route[i + 1].Lat}, {route[i + 1].Lon})");
                }

                if (distance < minDistanceMeters)
                {
                    minDistanceMeters = distance;
                }

                if (minDistanceMeters <= toleranceMeters)
                {
                    return false;
                }
            }

            return minDistanceMeters > toleranceMeters;
        }

        public static bool HasArrived(double currentLat, double currentLon, double destLat, double destLon, double radiusMeters = 500)
        {
            double distanceKm = CalculateHaversineDistanceKm(currentLat, currentLon, destLat, destLon);

            double distanceMeters = distanceKm * 1000;

            return distanceMeters <= radiusMeters;
        }

        private static double CalculateHaversineDistanceKm(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            var dLat = ToRadians(latitude2 - latitude1);
            var dLon = ToRadians(longitude2 - longitude1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(latitude1)) * Math.Cos(ToRadians(latitude2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiumKm * c;
        }

        private static double DistanceToSegmentMeters(double px, double py, double ax, double ay, double bx, double by)
        {
            double latToMeters = 111320.0;
            double lonToMeters = 111320.0 * Math.Cos(ToRadians((ax + bx) / 2));

            double pXm = py * lonToMeters;
            double pYm = px * latToMeters;

            double aXm = ay * lonToMeters;
            double aYm = ax * latToMeters;

            double bXm = by * lonToMeters;
            double bYm = bx * latToMeters;

            double abX = bXm - aXm;
            double abY = bYm - aYm;

            double apX = pXm - aXm;
            double apY = pYm - aYm;

            double abSquared = (abX * abX) + (abY * abY);
            if (abSquared == 0) return Math.Sqrt((apX * apX) + (apY * apY));

            double t = ((apX * abX) + (apY * abY)) / abSquared;

            t = Math.Max(0, Math.Min(1, t));

            double cX = aXm + t * abX;
            double cY = aYm + t * abY;

            double distanceX = pXm - cX;
            double distanceY = pYm - cY;

            return Math.Sqrt((distanceX * distanceX) + (distanceY * distanceY));
        }

        private static double ToRadians(double angleInDegrees)
        {
            return (angleInDegrees * Math.PI) / 180;
        }
    }
}
