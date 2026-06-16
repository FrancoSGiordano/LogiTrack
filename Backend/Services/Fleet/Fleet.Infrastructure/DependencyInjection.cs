
using Fleet.Application.Interfaces;
using Fleet.Infrastructure.Persistence;
using Fleet.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fleet.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FleetDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("FleetDbConnection"));
            });

            services.AddScoped<ITruckRepository, TruckRepository>();
            services.AddScoped<ITripRepository, TripRepository>();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["REDIS_CONNECTION"] ?? "localhost:6379";
            });

            return services;
        }
    }
}
