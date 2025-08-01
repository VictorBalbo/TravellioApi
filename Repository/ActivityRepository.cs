using Microsoft.EntityFrameworkCore;
using Travellio.DbContexts;
using Travellio.Models;

namespace Travellio.Repository;

public class ActivityRepository(AppDbContext context) : BaseRepository<Activity>(context), IActivityRepository
{
    //TODO: Check for user
    public async Task<IEnumerable<Activity>> GetAllAsync(Guid tripId, Guid destinationId)
    {
        return await _dbSet
            .Where(a => a.DestinationId == destinationId && a.Destination.TripId == tripId)
            .ToListAsync();
    }

    public async Task<Activity?> GetByIdAsync(Guid tripId, Guid destinationId, Guid id)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a =>
                a.DestinationId == destinationId &&
                a.Destination.TripId == tripId &&
                a.Id == id);
    }
}
