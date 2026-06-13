using Microsoft.EntityFrameworkCore;
using Travellio.Airports.Services;
using Travellio.Infrastructure.DbContexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("SqlConnectionString"))
        .UseSnakeCaseNamingConvention());

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