using BuildingBlocks.Extensions;
using BuildingBlocks.Interfaces;
using BuildingBlocks.Messaging;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using StackExchange.Redis;
using Tracking.Application.Interfaces;
using Tracking.Application.Services;
using Tracking.Infrastructure.Caching;
using Tracking.Infrastructure.Messaging;
using Tracking.Infrastructure.Persistance;
using Tracking.Infrastructure.Repositories;
using Tracking.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<TrackingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TrackingDbConnection")));

builder.Services.AddScoped<ITruckPositionRepository, TruckPositionRepository>();
builder.Services.AddScoped<ITruckPositionService, TruckPositionService>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["REDIS_CONNECTION"] ?? "localhost:6379";
});

builder.Services.AddCustomMassTransit();

builder.Services.AddScoped<IEventBus, MassTransitEventBus>();

builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    return new ConnectionFactory()
    {
        HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost",
        Port = 5672,
        UserName = "admin",
        Password = "admin",
        VirtualHost = "/"
    };
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect(builder.Configuration["REDIS_CONNECTION"] ?? "localhost:6379");
});
builder.Services.AddSingleton<ILiveTrackingPublisher, RedisLiveTrackingPublisher>();

builder.Services.AddSingleton<ITripRouteCache, RedisTripRouteCache>();

var host = builder.Build();
host.Run();
