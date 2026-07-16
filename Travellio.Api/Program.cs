using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using StackExchange.Redis;
using Travellio.Api.Converters;
using Travellio.Api.Middleware;
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
builder.Services.AddR2Infrastructure(builder.Configuration);

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
builder.Services.AddScoped<IAirportRepository, AirportRepository>();
builder.Services.AddScoped<IPlaceService, PlaceService>();
builder.Services.AddScoped<WanderlogProvider>();
builder.Services.AddScoped<GooglePlacesProvider>();
builder.Services.AddScoped<IPlaceProvider, FallbackPlaceProvider>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IDestinationService, DestinationService>();
builder.Services.AddScoped<ITransportationService, TransportationService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<IAccommodationService, AccommodationService>();
builder.Services.AddScoped<IImageStorageService, ImageStorageService>();

// Add Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddHttpClient();

// Surface model validation failures in the Serilog request log line
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    var defaultFactory = options.InvalidModelStateResponseFactory;
    options.InvalidModelStateResponseFactory = context =>
    {
        var diagnosticContext = context.HttpContext.RequestServices.GetRequiredService<IDiagnosticContext>();
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(entry => entry.Key, entry => entry.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

        diagnosticContext.Set("ValidationErrors", errors, destructureObjects: true);

        return defaultFactory(context);
    };
});

// Add Loggers
builder.Host.AddSerilog();

// Build
var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
app.AddRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();
app.MapControllers();

app.Run();