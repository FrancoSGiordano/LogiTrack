using BuildingBlocks.CQRS;
using Fleet.Application.DTOs.Trip;

namespace Fleet.Application.Trips.Commands.UpdateTripStatus
{
    public record UpdateTripStatusCommand(Guid Id, string Status) : ICommand<TripResponse>;
    
}
