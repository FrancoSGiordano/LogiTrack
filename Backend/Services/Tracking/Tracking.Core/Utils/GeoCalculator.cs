namespace Tracking.Core.Utils
{
    public static class GeoCalculator
    {
        private const double EarthRadiumKm = 6971.0;

        public static double GetDistance(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            var dLat = ToRadians(latitude2 - latitude1);
            var dLon = ToRadians(longitude2 - longitude1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(latitude1)) * Math.Cos(ToRadians(latitude2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiumKm * c;
        }

        private static double ToRadians(double angleInDegrees)
        {
            return (angleInDegrees * Math.PI) / 180;
        }
    }
}
