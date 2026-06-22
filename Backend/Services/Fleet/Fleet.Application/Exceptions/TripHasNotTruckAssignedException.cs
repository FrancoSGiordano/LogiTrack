namespace Fleet.Application.Exceptions
{
    public class TripHasNotTruckAssignedException : Exception
    {
        public TripHasNotTruckAssignedException(Guid Id) : base($"El viaje con ID: {Id} no tiene un camión asignado.") { }
    }
}
