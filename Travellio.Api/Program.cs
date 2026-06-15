using System.Text.Json.Serialization;
using StackExchange.Redis;
using Travellio.Api.Queries;
using Travellio.Api.Repositories;
using Travellio.Api.Services;
using Travellio.Api.Services.PlaceProviders;
using Travellio.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Database connections
builder.Services.AddDatabaseInfrastructure(builder.Configuration);
builder.Services.AddRedisInfrastructure(builder.Configuration);

// Add Dependency Injection
builder.Services.AddScoped<ICachedPlaceProvider>(sp =>
{
    var redis = sp.GetService<IConnectionMultiplexer>();
    var logger = sp.GetRequiredService<ILogger<CachedProvider>>();

    if (redis is null)
    {
        return new NoCacheProvider();
    }

    return new CachedProvider(redis, logger);
});
builder.Services.AddScoped<ITripRepository, TripRepository>();
builder.Services.AddScoped<ITripQuery, TripQuery>();
builder.Services.AddScoped<IDestinationRepository, DestinationRepository>();
builder.Services.AddScoped<IDestinationQuery, DestinationQuery>();
builder.Services.AddScoped<ITransportationRepository, TransportationRepository>();
builder.Services.AddScoped<ITransportationQuery, TransportationQuery>();
builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
builder.Services.AddScoped<IActivityQuery, ActivityQuery>();
builder.Services.AddScoped<IAccommodationRepository, AccommodationRepository>();
builder.Services.AddScoped<IAccommodationQuery, AccommodationQuery>();
builder.Services.AddScoped<IPlaceService, PlaceService>();
builder.Services.AddScoped<IPlaceProvider, WanderlogProvider>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IDestinationService, DestinationService>();
builder.Services.AddScoped<ITransportationService, TransportationService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<IAccommodationService, AccommodationService>();

// Add Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddHttpClient();

// Add Loggers
builder.Host.AddSerilog();

// Build
var app = builder.Build();

app.AddRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();
app.MapControllers();

app.Run();