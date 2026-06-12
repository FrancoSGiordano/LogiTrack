

namespace Fleet.Application.Exceptions
{
    public class TripNotFoundException : Exception
    {
        public TripNotFoundException(Guid Id) : base($"El viaje con ID: {Id} no fue encontrado."){ }
    }
}
