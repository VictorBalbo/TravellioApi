using Microsoft.EntityFrameworkCore;
using Travellio.DbContexts;
using Travellio.Models;
using Travellio.Services;

namespace Travellio.Repository;

public class TripRepository(AppDbContext context, IPlaceService placeService) : BaseRepository<Trip>(context), ITripRepository
{
    private readonly IPlaceService _placeService = placeService;

    public async Task<IEnumerable<Trip>> GetAllAsync()
    {
        var trips = await _dbSet.ToListAsync();
        return trips;
    }

    public async Task<Trip?> GetByIdAsync(Guid id)
    {
        var trip = await _dbSet
            .Include(t => t.Destinations)
            .Include(t => t.Transportations!)
                .ThenInclude(tr => tr.Segments)
            .FirstOrDefaultAsync(t => t.Id == id);

         if (trip != null)
        {
            trip = await EnrichTripWithPlaceDetailsAsync(trip);
        }

        return trip;
    }

    /// <summary>
    /// Enrich Trip with Place Details for Destinations and Transportations.
    /// </summary>
    /// <param name="trip"></param>
    /// <returns></returns>
    private async Task<Trip> EnrichTripWithPlaceDetailsAsync(Trip trip)
    {
        var getPlaceTasks = new List<Task>();
        if(trip.Destinations?.Any() == true)
        {
            var destinationTasks = trip.Destinations.Select(async d => d.Place = await _placeService.GetPlaceDetails(d.PlaceId));
            getPlaceTasks.AddRange(destinationTasks);
        }
        if (trip.Transportations?.Any() == true)
        {
            var transportationTasks = trip.Transportations.SelectMany(t =>
            t.Segments.Select(async s =>
            {
                var originTerminalTask = _placeService.GetPlaceDetails(s.OriginTerminalPlaceId);
                var DestinationTerminalTask = _placeService.GetPlaceDetails(s.DestinationTerminalPlaceId);
                await Task.WhenAll(originTerminalTask, DestinationTerminalTask);
                s.OriginTerminal = await originTerminalTask;
                s.DestinationTerminal = await = DestinationTerminalTask;
            })
        );
            getPlaceTasks.AddRange(transportationTasks);
        }
        
        await Task.WhenAll(getPlaceTasks);
        return trip;
    }
}
