using Microsoft.EntityFrameworkCore;
using Travellio.DbContexts;
using Travellio.Models;

namespace Travellio.Repository;

public class DestinationRepository(AppDbContext context) : BaseRepository<Destination>(context), IDestinationRepository
{
    //TODO: Check for user
    public async Task<IEnumerable<Destination>> GetAllAsync(Guid tripId)
    {
        return await _dbSet.Where(d => d.TripId == tripId).ToListAsync();
    }

    public async Task<Destination?> GetByIdAsync(Guid tripId, Guid id)
    {
        return await _dbSet
            .Include(d => d.Accommodations)
            .Include(d => d.Activities)
            .FirstOrDefaultAsync(d => d.TripId == tripId && d.Id == id);
    }
}