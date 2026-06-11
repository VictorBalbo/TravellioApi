using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StackExchange.Redis;
using TravellioApi.DbContexts;
using TravellioApi.Repositories;
using TravellioApi.Services;
using TravellioApi.Services.PlaceProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Database connections
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var sqlConnectionString = builder.Configuration.GetValue<string>("SqlConnectionString");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(sqlConnectionString).UseSnakeCaseNamingConvention());

var redisConnectionString = builder.Configuration.GetValue<string>("RedisConnectionString");
if (!string.IsNullOrEmpty(redisConnectionString))
{
    builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
        ConnectionMultiplexer.Connect(redisConnectionString));
    builder.Services.AddScoped<ICachedPlaceProvider, InternalProvider>();
}
else
{
    builder.Services.AddScoped<ICachedPlaceProvider, NullCachedPlaceProvider>();
}

// Add Dependency Injection
builder.Services.AddScoped<ITripRepository, TripRepository>();
builder.Services.AddScoped<IDestinationRepository, DestinationRepository>();
builder.Services.AddScoped<ITransportationRepository, TransportationRepository>();
builder.Services.AddScoped<IPlaceService, PlaceService>();
builder.Services.AddScoped<IPlaceProvider, WanderlogProvider>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IDestinationService, DestinationService>();
builder.Services.AddScoped<ITransportationService, TransportationService>();

// Add Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
builder.Services.AddHttpClient();

// Add Loggers
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();

// Build
var app = builder.Build();

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        var endpoint = httpContext.GetEndpoint() as RouteEndpoint;
        var routePattern = endpoint?.RoutePattern.RawText ?? httpContext.Request.Path;

        diagnosticContext.Set("RouteTemplate", routePattern);
    };
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();
app.MapControllers();

app.Run();