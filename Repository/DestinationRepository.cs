using Microsoft.EntityFrameworkCore;
using Travellio.DbContexts;
using Travellio.Models;
using Travellio.Services;

namespace Travellio.Repository;

public class DestinationRepository(AppDbContext context, IPlaceService placeService) : BaseRepository<Destination>(context), IDestinationRepository
{
    private readonly IPlaceService _placeService = placeService;

    //TODO: Check for user
    public async Task<IEnumerable<Destination>> GetAllAsync(Guid tripId)
    {
        return await _dbSet.Where(d => d.TripId == tripId).ToListAsync();
    }

    public async Task<Destination?> GetByIdAsync(Guid tripId, Guid id)
    {
        var destination = await _dbSet
            .Include(d => d.Accommodations)
            .Include(d => d.Activities)
            .FirstOrDefaultAsync(d => d.TripId == tripId && d.Id == id);

        if (destination != null)
        {
            destination = await EnrichDestinationWithPlaceDetailsAsync(destination);
        }
        return destination;
    }

    /// <summary>
    /// Enrich Trip with Place Details for Destinations and Transportations.
    /// </summary>
    /// <param name="trip"></param>
    /// <returns></returns>
    private async Task<Destination> EnrichDestinationWithPlaceDetailsAsync(Destination destination)
    {
        var getPlaceTasks = new List<Task>();

        var destinationTask = _placeService.GetPlaceDetails(destination.PlaceId);
        getPlaceTasks.Add(destinationTask);

        if (destination.Accommodations?.Any() == true)
        {
            var accommodationsTasks = destination.Accommodations.Select(async a => a.Place = await _placeService.GetPlaceDetails(a.PlaceId));
            getPlaceTasks.AddRange(accommodationsTasks);
        }

        if (destination.Activities?.Any() == true)
        {
            var activitiesTasks = destination.Activities.Select(async a => a.Place = await _placeService.GetPlaceDetails(a.PlaceId));
            getPlaceTasks.AddRange(activitiesTasks);
        }

        await Task.WhenAll(getPlaceTasks);
        destination.Place = await destinationTask;

        return destination;
    }
}