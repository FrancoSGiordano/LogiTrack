using BuildingBlocks.Messaging.Events;
using Fleet.Application.Trips.Commands.UpdateTripStatus;
using MassTransit;
using MediatR;

namespace Fleet.Infrastructure.Messaging.Consumers
{
    public class TripCompletedEventConsumer : IConsumer<TripCompletedEvent>
    {
        private readonly IMediator mediator;

        public TripCompletedEventConsumer(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task Consume(ConsumeContext<TripCompletedEvent> context)
        {
            var message = context.Message;

            var status = "Completed";

            var command = new UpdateTripStatusCommand(message.TripId, status, message.FinishLat, message.FinishLon);

            await mediator.Send(command);
        }
    }
}
