using Microsoft.EntityFrameworkCore;
using Travellio.Domain.Entities;
using Travellio.Infrastructure.DbContexts;

namespace Travellio.Api.Repositories;

public class AirportRepository(AppDbContext context, ILogger<AirportRepository> logger) : IAirportRepository
{
    private readonly DbSet<Airport> _dbSet = context.Airports;

    public async Task<string?> GetIataCodeByCoordinatesAsync(decimal lat, decimal lng)
    {
        const decimal tolerance = 0.5m;
        var iataCode = await _dbSet
            .Where(a => Math.Abs(a.Lat - lat) < tolerance && Math.Abs(a.Lng - lng) < tolerance)
            .OrderBy(a => Math.Abs(a.Lat - lat) + Math.Abs(a.Lng - lng))
            .Select(a => a.IataCode)
            .FirstOrDefaultAsync();

        if (iataCode is null)
            logger.LogWarning("Airport code not found for coordinates: {Lat}, {Lng}", lat, lng);

        return iataCode;
    }
}