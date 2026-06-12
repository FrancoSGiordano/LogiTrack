using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Tracking.Application.Interfaces;
using Tracking.Application.Services;
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
    options.InstanceName = "LogiTrack_";
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect(builder.Configuration["REDIS_CONNECTION"] ?? "localhost:6379");
});

var host = builder.Build();
host.Run();
