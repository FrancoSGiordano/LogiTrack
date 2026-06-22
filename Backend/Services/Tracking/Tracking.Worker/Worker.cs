using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Tracking.Application.DTOs;
using Tracking.Application.Interfaces;


namespace Tracking.Worker
{
    public class Worker : BackgroundService
    {
        private readonly IConnectionFactory connectionFactory;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<Worker> logger;

        public Worker(IConnectionFactory connectionFactory, IServiceScopeFactory scopeFactory, ILogger<Worker> logger)
        {
            this.connectionFactory = connectionFactory;
            this.scopeFactory = scopeFactory;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var connection = await connectionFactory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "truck_pings",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    var ping = JsonSerializer.Deserialize<TruckPingReceived>(message);

                    if(ping != null)
                    {
                        using var scope = scopeFactory.CreateScope();
                        var truckPositionService = scope.ServiceProvider.GetRequiredService<ITruckPositionService>();

                        await truckPositionService.AddTruckPosition(ping);
                    }

                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error al procesar el mensaje de telemetría.");
                    await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            await channel.BasicConsumeAsync(queue: "truck_pings", autoAck: false, consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
