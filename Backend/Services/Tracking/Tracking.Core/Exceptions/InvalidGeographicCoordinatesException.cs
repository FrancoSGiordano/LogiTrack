namespace Tracking.Core.Exceptions
{
    public class InvalidGeographicCoordinatesException : Exception
    {
        public InvalidGeographicCoordinatesException(double latitude, double longitude) : base($"La coordenadas geográficas recibidas son invalidas: \"Latitud: {latitude}\" | \"Longitud: {longitude}\"") { }
    }
}
