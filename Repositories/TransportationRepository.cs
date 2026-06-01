using Microsoft.EntityFrameworkCore;
using TravellioApi.DbContexts;
using TravellioApi.Models;
using TravellioApi.Services;

namespace TravellioApi.Repositories;

public class TransportationRepository(AppDbContext context, IPlaceService placeService)
    : BaseRepository<Transportation>(context), ITransportationRepository
{
    public async Task<ICollection<Transportation>> GetAllAsync(Guid tripId, CancellationToken cancellationToken)
    {
        return await DbSet.Where(destination => destination.TripId == tripId).ToListAsync(cancellationToken);
    }

    public async Task<Transportation?> GetByIdAsync(Guid tripId, Guid id, CancellationToken cancellationToken)
    {
        var transportation =
            await DbSet
                .Where(d => d.TripId == tripId && d.Id == id)
                .Include(d => d.Legs)
                .Include(d => d.Arrival)
                .Include(d => d.Departure)
                .FirstOrDefaultAsync(cancellationToken);

        if (transportation != null)
        {
            transportation = await EnrichDestinationWithPlaceDetailsAsync(transportation, cancellationToken);
        }

        return transportation;
    }

    /// <summary>
    /// Enrich Transportation with Place Details for the Destination and Origin.
    /// </summary>
    /// <param name="transportation"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<Transportation> EnrichDestinationWithPlaceDetailsAsync(Transportation transportation,
        CancellationToken cancellationToken)
    {
        var getPlaceTasks = new List<Task>();

        var originTask = Task.FromResult<Place?>(null);
        var destinationTask = Task.FromResult<Place?>(null);;
        if (transportation.Arrival?.PlaceId != null)
        {
            originTask = placeService.GetPlaceDetails(transportation.Arrival.PlaceId, cancellationToken);
            getPlaceTasks.Add(originTask);
        }

        if (transportation.Departure?.PlaceId != null)
        {
            destinationTask = placeService.GetPlaceDetails(transportation.Departure.PlaceId, cancellationToken);
            getPlaceTasks.Add(destinationTask);
        }
        
        await Task.WhenAll(getPlaceTasks);
        transportation.Arrival?.Place = await originTask;
        transportation.Departure?.Place = await destinationTask;

        return transportation;
    }
}