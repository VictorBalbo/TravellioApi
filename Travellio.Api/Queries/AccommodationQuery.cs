using Microsoft.EntityFrameworkCore;
using Travellio.Api.Services;
using Travellio.Domain.DTOs;
using Travellio.Domain.Entities;
using Travellio.Infrastructure.DbContexts;

namespace Travellio.Api.Queries;

public class AccommodationQuery(AppDbContext context, IPlaceService placeService) : IAccommodationQuery
{
    private readonly DbSet<Accommodation> _dbSet = context.Set<Accommodation>();

    public async Task<ICollection<AccommodationDto>> GetAllAsync(Guid destinationId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.DestinationId == destinationId)
            .Select(a => new AccommodationDto
            {
                Id = a.Id,
                Name = a.Name,
                PlaceId = a.PlaceId,
                Coordinates = new Coordinates { Lat = a.Coordinates.Lat, Lng = a.Coordinates.Lng },
                Address = a.Address,
                CheckIn = a.CheckIn,
                CheckOut = a.CheckOut,
                ImageUrl = a.ImageUrl,
                Website = a.Website,
                Notes = a.Notes,
                Price = a.Price == null ? null : new PriceDto(a.Price.Value, a.Price.Currency),
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<AccommodationDto?> GetByIdAsync(Guid destinationId, Guid id, bool enrichPlaces, CancellationToken cancellationToken)
    {
        var accommodation = await _dbSet
            .AsNoTracking()
            .Where(a => a.DestinationId == destinationId && a.Id == id)
            .Select(a => new AccommodationDto
            {
                Id = a.Id,
                Name = a.Name,
                PlaceId = a.PlaceId,
                Coordinates = new Coordinates { Lat = a.Coordinates.Lat, Lng = a.Coordinates.Lng },
                Address = a.Address,
                CheckIn = a.CheckIn,
                CheckOut = a.CheckOut,
                ImageUrl = a.ImageUrl,
                Website = a.Website,
                Notes = a.Notes,
                Price = a.Price == null ? null : new PriceDto(a.Price.Value, a.Price.Currency),
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (accommodation != null && enrichPlaces)
            accommodation.Place = await placeService.GetPlaceDetails(accommodation.PlaceId, cancellationToken);

        return accommodation;
    }
}
