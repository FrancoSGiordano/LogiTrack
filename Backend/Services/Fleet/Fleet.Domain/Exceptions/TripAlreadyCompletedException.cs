namespace Fleet.Domain.Exceptions
{
    public class TripAlreadyCompletedException : Exception
    {
        public TripAlreadyCompletedException(Guid Id, Guid TruckId) : base($"El viaje con ID: \"{Id}\" realizado por el camión \"{TruckId}\" ya fue completado.")
        {

        }
    }
}
