
namespace Fleet.Application.Exceptions
{
    public class InvalidTripStatusException : Exception
    { 
        public InvalidTripStatusException(string Status) : base($"El estado de viaje: {Status} es invalido. Utilice: InProgress o Completed.") { }
    }
}
