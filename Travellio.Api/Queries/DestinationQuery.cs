using Microsoft.EntityFrameworkCore;
using Travellio.Api.Services;
using Travellio.Domain.DTOs;
using Travellio.Domain.Entities;
using Travellio.Infrastructure.DbContexts;

namespace Travellio.Api.Queries;

public class DestinationQuery(AppDbContext context, IPlaceService placeService) : IDestinationQuery
{
    private readonly DbSet<Destination> _dbSet = context.Set<Destination>();

    public async Task<ICollection<DestinationDto>> GetAllAsync(Guid tripId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(d => d.TripId == tripId)
            .Select(d => new DestinationDto
            {
                Id = d.Id,
                PlaceId = d.PlaceId,
                Name = d.Name,
                Coordinates = d.Coordinates,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                Notes = d.Notes,
                ActivitiesCount = d.Activities!.Count,
                AccommodationsCount = d.Accommodations!.Count,
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<DestinationDto?> GetByIdAsync(Guid tripId, Guid id, bool enrichPlaces,
        CancellationToken cancellationToken)
    {
        var destination = await _dbSet
            .AsNoTracking()
            .Where(d => d.TripId == tripId && d.Id == id)
            .Select(d => new DestinationDto
            {
                Id = d.Id,
                PlaceId = d.PlaceId,
                Name =  d.Name,
                Coordinates =  d.Coordinates,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                Notes = d.Notes,
                ActivitiesCount = d.Activities!.Count,
                AccommodationsCount = d.Accommodations!.Count,
                Activities = d.Activities!.Select(a => new ActivityDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    PlaceId = a.PlaceId,
                    Address =  a.Address,
                    Coordinates =  a.Coordinates,
                    ScheduledAt = a.ScheduledAt,
                    TicketRequired = a.TicketRequired,
                    TicketPurchased = a.TicketPurchased,
                    Price = a.Price == null ? null : new PriceDto(a.Price.Value, a.Price.Currency),
                    Website = a.Website,
                    Notes = a.Notes,
                }).ToList(),
                Accommodations = d.Accommodations!.Select(a => new AccommodationDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Address =  a.Address,
                    Coordinates =  a.Coordinates,
                    PlaceId = a.PlaceId,
                    CheckIn = a.CheckIn,
                    CheckOut = a.CheckOut,
                    ImageUrl = a.ImageUrl,
                    Website = a.Website,
                    Notes = a.Notes,
                    Price = a.Price == null ? null : new PriceDto(a.Price.Value, a.Price.Currency),
                }).ToList(),
            })
            .AsSplitQuery()
            .FirstOrDefaultAsync(cancellationToken);

        if (destination != null && enrichPlaces)
        {
            destination = await EnrichPlaceDetailsAsync(destination, cancellationToken);
        }

        return destination;
    }

    private async Task<DestinationDto> EnrichPlaceDetailsAsync(DestinationDto destination,
        CancellationToken cancellationToken)
    {
        var destinationPlaceTask = placeService.GetPlaceDetails(destination.PlaceId, cancellationToken);

        var accommodationTasks = (destination.Accommodations ?? [])
            .Select(async a => a.Place = await placeService.GetPlaceDetails(a.PlaceId, cancellationToken));

        var activityTasks = (destination.Activities ?? [])
            .Select(async a => a.Place = await placeService.GetPlaceDetails(a.PlaceId, cancellationToken));

        await Task.WhenAll([destinationPlaceTask, ..accommodationTasks, ..activityTasks]);

        destination.Place = await destinationPlaceTask;

        return destination;
    }
}