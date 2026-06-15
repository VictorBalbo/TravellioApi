using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using Travellio.Domain.Entities;
using Travellio.Infrastructure.DbContexts;

namespace Travellio.Airports.Services;

public class AirportImporter(AppDbContext dbContext, ILogger<AirportImporter> logger)
{
    private readonly DbSet<Airport> _dbSet = dbContext.Airports;

    public async Task ImportAsync(string filePath, CancellationToken ct = default)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
            BadDataFound = null,
        });

        var airports = new List<Airport>();

        await foreach (var record in csv.GetRecordsAsync<AirportCsvRecord>(ct))
        {
            if (string.IsNullOrWhiteSpace(record.IataCode))
            {
                continue;
            }

            airports.Add(new Airport
            {
                Name = record.Name,
                Lat = record.LatitudeDeg,
                Lng = record.LongitudeDeg,
                CountryCode = record.IsoCountry,
                City = string.IsNullOrWhiteSpace(record.Municipality) ? null : record.Municipality,
                IataCode = record.IataCode.Trim(),
            });
        }

        var existingCodes = await _dbSet
            .Select(a => a.IataCode)
            .ToHashSetAsync(StringComparer.OrdinalIgnoreCase, ct);

        var newAirports = airports
            .Where(a => !existingCodes.Contains(a.IataCode))
            .ToList();

        if (newAirports.Count == 0)
        {
            logger.LogInformation("No new airports to import");
            return;
        }

        await _dbSet.AddRangeAsync(newAirports, ct);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Imported {Count} airports", newAirports.Count);
    }
}

file sealed class AirportCsvRecord
{
    [Name("name")] public string Name { get; init; } = "";

    [Name("latitude_deg")] public double LatitudeDeg { get; init; }

    [Name("longitude_deg")] public double LongitudeDeg { get; init; }

    [Name("iso_country")] public string IsoCountry { get; init; } = "";

    [Name("municipality")] public string Municipality { get; init; } = "";

    [Name("icao_code")] public string IcaoCode { get; init; } = "";

    [Name("iata_code")] public string IataCode { get; init; } = "";
}