using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BuildingBlocks.Extensions
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection AddCustomMassTransit(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddMassTransit(x =>
            {
                if (assemblies.Length > 0)
                {
                    foreach(var assembly in assemblies.Distinct()) 
                    {
                        x.AddConsumers(assembly);
                    }
                }

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost", "/", h =>
                    {
                        h.Username("admin");
                        h.Password("admin");
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
