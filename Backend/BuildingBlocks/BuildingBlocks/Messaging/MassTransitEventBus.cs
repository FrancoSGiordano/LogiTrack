using BuildingBlocks.Interfaces;
using MassTransit;

namespace BuildingBlocks.Messaging
{
    public class MassTransitEventBus : IEventBus
    {
        private readonly IPublishEndpoint publishEndpoint;

        public MassTransitEventBus(IPublishEndpoint publishEndpoint)
        {
            this.publishEndpoint = publishEndpoint;
        }

        public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
        {
            await publishEndpoint.Publish(@event, cancellationToken);
        }
    }
}
