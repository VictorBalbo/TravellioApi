using Travellio.Airports.Services;
using Travellio.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Add Database connections
builder.Services.AddDatabaseInfrastructure(builder.Configuration);

// Add Dependency Injection
builder.Services.AddScoped<AirportImporter>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();

app.MapPost("/import", async (AirportImporter importer, string filePath) =>
{
    await importer.ImportAsync(filePath);
    return Results.Ok();
});

app.Run();