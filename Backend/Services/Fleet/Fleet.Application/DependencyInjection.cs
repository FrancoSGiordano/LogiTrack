using BuildingBlocks.Interfaces;
using BuildingBlocks.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Fleet.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddScoped<IEventBus, MassTransitEventBus>();

            return services;
        }
    }
}
