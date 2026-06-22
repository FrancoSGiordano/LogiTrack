using BuildingBlocks.Extensions;
using Fleet.Application;
using Fleet.Application.Interfaces;
using Fleet.Infrastructure;
using Fleet.Infrastructure.Messaging.Consumers;
using Fleet.Infrastructure.Services;
using Microsoft.OpenApi;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LogiTrack - Fleet API",
        Version = "v1",
        Description = "Microservicio para la gestión de vehículos y geocercas logísticas.",
        Contact = new OpenApiContact
        {
            Name = "Franco Giordano",
            Email = "fransantino03@gmail.com"
        }
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddHttpClient<IRoutingService, OsrmRoutingService>();

builder.Services.AddCustomMassTransit(
    typeof(TripCompletedEventConsumer).Assembly,
    typeof(RouteMissingInCacheEventConsumer).Assembly 
);

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

builder.Services.AddHttpClient();

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Fleet API v1");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowFrontend");

app.UseAuthorization();
app.MapControllers();

app.Run();
