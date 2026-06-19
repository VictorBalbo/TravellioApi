using Microsoft.EntityFrameworkCore;
using Travellio.Domain.Entities;
using Travellio.Infrastructure.DbContexts;

namespace Travellio.Api.Repositories;

public class AirportRepository(AppDbContext context, ILogger<AirportRepository> logger) : IAirportRepository
{
    private readonly DbSet<Airport> _dbSet = context.Airports;

    public async Task<string?> GetIataCodeByCoordinatesAsync(Coordinates coordinates)
    {
        const decimal tolerance = 0.5m;
        var iataCode = await _dbSet
            .Where(a => Math.Abs(a.Lat - coordinates.Lat) < tolerance && Math.Abs(a.Lng - coordinates.Lng) < tolerance)
            .OrderBy(a => Math.Abs(a.Lat - coordinates.Lat) + Math.Abs(a.Lng - coordinates.Lng))
            .Select(a => a.IataCode)
            .FirstOrDefaultAsync();

        if (iataCode is null)
            logger.LogWarning("Airport code not found for coordinates: {Lat}, {Lng}", coordinates.Lat, coordinates.Lng);

        return iataCode;
    }
}