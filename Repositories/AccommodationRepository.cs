using Microsoft.EntityFrameworkCore;
using TravellioApi.DbContexts;
using TravellioApi.Models.Entities;

namespace TravellioApi.Repositories;

public class AccommodationRepository(AppDbContext context)
    : BaseRepository<Accommodation>(context), IAccommodationRepository
{
    public async Task<ICollection<Accommodation>> GetAllAsync(Guid destinationId, CancellationToken cancellationToken)
    {
        return await DbSet.Where(a => a.DestinationId == destinationId).ToListAsync(cancellationToken);
    }
}
