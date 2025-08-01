using Microsoft.EntityFrameworkCore;
using Travellio.DbContexts;
using Travellio.Models;

namespace Travellio.Repository;

public class TripRepository(AppDbContext context) : BaseRepository<Trip>(context), ITripRepository
{
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
        return trip;
    }
}
