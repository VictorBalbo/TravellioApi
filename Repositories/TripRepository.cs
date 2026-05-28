using Microsoft.EntityFrameworkCore;
using TravellioApi.DbContexts;
using TravellioApi.Models;
using TravellioApi.Services;

namespace TravellioApi.Repositories;

public class TripRepository(AppDbContext context, IPlaceService placeService) : BaseRepository<Trip>(context), ITripRepository
{
    public async Task<IEnumerable<Trip>> GetAllAsync(CancellationToken cancellationToken)
    {
        var trips = await DbSet.ToListAsync(cancellationToken);
        return trips;
    }

    public override async Task<Trip?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var trip = await DbSet
            .Include(t => t.Destinations)
            .Include(t => t.Transportations!)
            .ThenInclude(tr => tr.Segments)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (trip != null)
        {
            trip = await EnrichTripWithPlaceDetailsAsync(trip, cancellationToken);
        }

        return trip;
    }
    
    /// <summary>
    /// Enrich Trip with Place Details for Destinations and Transportations.
    /// </summary>
    /// <param name="trip"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<Trip> EnrichTripWithPlaceDetailsAsync(Trip trip, CancellationToken cancellationToken)
    {
        var getPlaceTasks = new List<Task>();
        if (trip.Destinations?.Count > 0)
        {
            var destinationTasks = trip.Destinations
                .Select(async d => d.Place = await placeService.GetPlaceDetails(d.PlaceId, cancellationToken));
            getPlaceTasks.AddRange(destinationTasks);
        }
        if (trip.Transportations?.Count > 0)
        {
            var transportationTasks = trip.Transportations.SelectMany(t =>
                t.Segments.Select(async s =>
                {
                    var originTerminalTask = placeService.GetPlaceDetails(s.OriginTerminalPlaceId, cancellationToken);
                    var destinationTerminalTask = placeService.GetPlaceDetails(s.DestinationTerminalPlaceId, cancellationToken);
                    await Task.WhenAll(originTerminalTask, destinationTerminalTask);
                    s.OriginTerminal = await originTerminalTask;
                    s.DestinationTerminal = await destinationTerminalTask;
                })
            );
            getPlaceTasks.AddRange(transportationTasks);
        }

        await Task.WhenAll(getPlaceTasks);
        return trip;
    }
}