using Microsoft.EntityFrameworkCore;
using Travellio.DbContexts;
using Travellio.Models;

namespace Travellio.Repository;

public class AccommodationRepository(AppDbContext context) : BaseRepository<Accommodation>(context), IAccommodationRepository
{
    //TODO: Check for user
    public async Task<IEnumerable<Accommodation>> GetAllAsync(Guid tripId, Guid destinationId)
    {
        return await _dbSet
            .Where(a => a.DestinationId == destinationId && a.Destination.TripId == tripId)
            .ToListAsync();
    }

    public async Task<Accommodation?> GetByIdAsync(Guid tripId, Guid destinationId, Guid id)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a =>
                a.DestinationId == destinationId &&
                a.Destination.TripId == tripId &&
                a.Id == id);
    }
}
