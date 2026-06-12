

namespace Fleet.Domain.Exceptions
{
    public class TruckUnavailableException : Exception
    {
        public TruckUnavailableException(Guid Id, string LicensePlate) : base($"El camión con ID: {Id} y patente: {LicensePlate} no esta disponible para iniciar un viaje.") { }
    }
}
