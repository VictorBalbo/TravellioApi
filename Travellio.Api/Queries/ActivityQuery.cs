using Microsoft.EntityFrameworkCore;
using Travellio.Api.Services;
using Travellio.Domain.DTOs;
using Travellio.Domain.Entities;
using Travellio.Infrastructure.DbContexts;

namespace Travellio.Api.Queries;

public class ActivityQuery(AppDbContext context, IPlaceService placeService) : IActivityQuery
{
    private readonly DbSet<Activity> _dbSet = context.Set<Activity>();

    public async Task<ICollection<ActivityDto>> GetAllAsync(Guid destinationId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.DestinationId == destinationId)
            .Select(a => new ActivityDto
            {
                Id = a.Id,
                Name = a.Name,
                PlaceId = a.PlaceId,
                Coordinates = new Coordinates { Lat = a.Coordinates.Lat, Lng = a.Coordinates.Lng },
                Address = a.Address,
                ScheduledAt = a.ScheduledAt,
                TicketRequired = a.TicketRequired,
                TicketPurchased = a.TicketPurchased,
                Price = a.Price == null ? null : new PriceDto(a.Price.Value, a.Price.Currency),
                Website = a.Website,
                Notes = a.Notes,
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ActivityDto?> GetByIdAsync(Guid destinationId, Guid id, bool enrichPlaces, CancellationToken cancellationToken)
    {
        var activity = await _dbSet
            .AsNoTracking()
            .Where(a => a.DestinationId == destinationId && a.Id == id)
            .Select(a => new ActivityDto
            {
                Id = a.Id,
                Name = a.Name,
                PlaceId = a.PlaceId,
                Coordinates = new Coordinates { Lat = a.Coordinates.Lat, Lng = a.Coordinates.Lng },
                Address = a.Address,
                ScheduledAt = a.ScheduledAt,
                TicketRequired = a.TicketRequired,
                TicketPurchased = a.TicketPurchased,
                Price = a.Price == null ? null : new PriceDto(a.Price.Value, a.Price.Currency),
                Website = a.Website,
                Notes = a.Notes,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (activity != null && enrichPlaces)
            activity.Place = await placeService.GetPlaceDetails(activity.PlaceId, cancellationToken);

        return activity;
    }
}
