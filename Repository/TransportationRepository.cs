using Microsoft.EntityFrameworkCore;
using Travellio.DbContexts;
using Travellio.Models;

namespace Travellio.Repository;

public class TransportationRepository(AppDbContext context) : BaseRepository<Transportation>(context), ITransportationRepository
{
    //TODO: Check for user
    public async Task<IEnumerable<Transportation>> GetAllAsync(Guid tripId)
    {
        return await _dbSet.Where(t => t.TripId == tripId).ToListAsync();
    }

    public async Task<Transportation?> GetByIdAsync(Guid tripId, Guid id)
    {
        return await _dbSet
            .Include(t => t.Segments)
            .FirstOrDefaultAsync(t => t.TripId == tripId && t.Id == id);
    }
}