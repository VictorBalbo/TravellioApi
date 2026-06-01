using Microsoft.EntityFrameworkCore;
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
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(sqlConnectionString).UseSnakeCaseNamingConvention());

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
builder.Services.AddScoped<IPlaceService, PlaceService>();
builder.Services.AddScoped<IPlaceProvider, WanderlogProvider>();

// Add Controllers
builder.Services.AddControllers();

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();
app.MapControllers();

app.Run();