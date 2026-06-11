using Microsoft.EntityFrameworkCore;
using TravellioApi.DbContexts;
using TravellioApi.Models.Entities;

namespace TravellioApi.Repositories;

public class TripRepository(AppDbContext context)
    : BaseRepository<Trip>(context), ITripRepository
{
    public async Task<ICollection<Trip>> GetAllAsync(CancellationToken cancellationToken)
    {
        var trips = await DbSet.ToListAsync(cancellationToken);
        return trips;
    }

    public override async Task<Trip?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var trip = await DbSet
            .Include(t => t.Destinations!)
            .ThenInclude(d => d.Activities)
            .Include(t => t.Transportations!)
            .ThenInclude(tr => tr.Legs)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        return trip;
    }
}