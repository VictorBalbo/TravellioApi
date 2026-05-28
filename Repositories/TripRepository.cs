using Microsoft.EntityFrameworkCore;
using TravellioApi.DbContexts;
using TravellioApi.Models;

namespace TravellioApi.Repositories;

public class TripRepository(AppDbContext context) : BaseRepository<Trip>(context), ITripRepository
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

        // if (trip != null)
        // {
        //     trip = await EnrichTripWithPlaceDetailsAsync(trip, cancellationToken);
        // }

        return trip;
    }
}