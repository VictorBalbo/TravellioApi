using Microsoft.EntityFrameworkCore;
using Travellio.Domain.Entities;
using Travellio.Infrastructure.DbContexts;

namespace Travellio.Api.Repositories;

public class TransportationRepository(AppDbContext context)
    : BaseRepository<Transportation>(context), ITransportationRepository
{
    public async Task<ICollection<Transportation>> GetAllAsync(Guid tripId, CancellationToken cancellationToken)
    {
        return await DbSet.Where(destination => destination.TripId == tripId).ToListAsync(cancellationToken);
    }

    public async Task<Transportation?> GetByIdAsync(Guid tripId, Guid id, CancellationToken cancellationToken)
    {
        var transportation =
            await DbSet
                .Where(d => d.TripId == tripId && d.Id == id)
                .Include(d => d.Legs)
                .Include(d => d.Arrival)
                .Include(d => d.Departure)
                .FirstOrDefaultAsync(cancellationToken);

        return transportation;
    }
}