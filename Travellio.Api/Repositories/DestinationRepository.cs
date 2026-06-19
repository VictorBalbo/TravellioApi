using Microsoft.EntityFrameworkCore;
using Travellio.Domain.Entities;
using Travellio.Infrastructure.DbContexts;

namespace Travellio.Api.Repositories;

public class DestinationRepository(AppDbContext context) : BaseRepository<Destination>(context), IDestinationRepository
{
    public async Task<ICollection<Destination>> GetAllAsync(Guid tripId, CancellationToken cancellationToken)
    {
        return await DbSet.Where(destination => destination.TripId == tripId).ToListAsync(cancellationToken);
    }

    public async Task<Destination?> GetByIdAsync(Guid tripId, Guid id, CancellationToken cancellationToken)
    {
        var destination =
            await DbSet
                .Where(d => d.TripId == tripId && d.Id == id)
                .Include(d => d.Accommodations)
                .Include(d => d.Activities)
                .FirstOrDefaultAsync(cancellationToken);

        return destination;
    }
}