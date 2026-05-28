using Microsoft.EntityFrameworkCore;
using TravellioApi.DbContexts;
using TravellioApi.Models;
using TravellioApi.Services;

namespace TravellioApi.Repositories;

public class DestinationRepository(AppDbContext context, IPlaceService placeService)
    : BaseRepository<Destination>(context), IDestinationRepository
{
    public async Task<ICollection<Destination>> GetAllAsync(Guid tripId, CancellationToken cancellationToken)
    {
        return await DbSet.Where(destination => destination.TripId == tripId).ToListAsync(cancellationToken);
    }

    public async Task<Destination?> GetByIdAsync(Guid tripId, Guid id, CancellationToken cancellationToken)
    {
        var destination =
            await DbSet
                .Where(d => d.TripId == tripId && d.Id == id)
                .Include(d => d.Accommodations)
                .Include(d => d.Activities)
                .FirstOrDefaultAsync(cancellationToken);

        if (destination != null)
        {
            destination = await EnrichDestinationWithPlaceDetailsAsync(destination, cancellationToken);
        }

        return destination;
    }

    /// <summary>
    /// Enrich Destination with Place Details for the Destination, Accommodations and Activities.
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<Destination> EnrichDestinationWithPlaceDetailsAsync(Destination destination,
        CancellationToken cancellationToken)
    {
        var getPlaceTasks = new List<Task>();

        var destinationTask = placeService.GetPlaceDetails(destination.PlaceId, cancellationToken);
        getPlaceTasks.Add(destinationTask);

        if (destination.Accommodations?.Count > 0)
        {
            var accommodationsTasks = destination.Accommodations
                .Select(async a => a.Place = await placeService.GetPlaceDetails(a.PlaceId, cancellationToken));
            getPlaceTasks.AddRange(accommodationsTasks);
        }

        if (destination.Activities?.Count > 0)
        {
            var activitiesTasks = destination.Activities.Select(async a =>
                a.Place = await placeService.GetPlaceDetails(a.PlaceId, cancellationToken));
            getPlaceTasks.AddRange(activitiesTasks);
        }

        await Task.WhenAll(getPlaceTasks);
        destination.Place = await destinationTask;

        return destination;
    }
}