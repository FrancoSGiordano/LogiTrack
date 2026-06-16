using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using RabbitMQ.Client;
using StackExchange.Redis;
using System.Text.Json;
using Tracking.API;
using Tracking.API.Hubs;
using Tracking.Application.DTOs;
using Tracking.Infrastructure.Persistance;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConnectionFactory>(sp => new ConnectionFactory
{
    HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost",
    UserName = "admin",
    Password = "admin"
});


builder.Services.AddDbContext<TrackingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TrackingDbConnection")));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["REDIS_CONNECTION"] ?? "localhost:6379";
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect(builder.Configuration["REDIS_CONNECTION"] ?? "localhost:6379");
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddSignalR();

builder.Services.AddHostedService<RedisSubscribeWorker>();

var app = builder.Build();

app.MapGet("/api/tracking/{truckId}/last-position", async (Guid truckId, IDistributedCache cache, TrackingDbContext dbContext) =>
{
    string cacheKey = $"last_pos_{truckId}";
    var cachedData = await cache.GetStringAsync(cacheKey);

    if (!string.IsNullOrEmpty(cachedData))
    {
        var ping = JsonSerializer.Deserialize<TruckPing>(cachedData);
        return Results.Ok(ping);
    }
    
    var lastPosition = await dbContext.TruckPositions
        .Where(p => p.TruckId == truckId)
        .OrderByDescending(p => p.Timestamp)
        .FirstOrDefaultAsync();

    return lastPosition is not null
        ? Results.Ok(lastPosition)
        : Results.NotFound(new { Message = $"No se encontraron posiciones para el camión {truckId}" });
});

app.UseCors("AllowFrontend");

app.UseWebSockets();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Tracking API v1");
    options.RoutePrefix = string.Empty;
});

app.MapHub<TrackingHub>("/hubs/tracking");

app.Run();


