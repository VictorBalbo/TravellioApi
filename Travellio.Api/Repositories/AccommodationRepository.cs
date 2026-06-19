using Microsoft.EntityFrameworkCore;
using Travellio.Domain.Entities;
using Travellio.Infrastructure.DbContexts;

namespace Travellio.Api.Repositories;

public class AccommodationRepository(AppDbContext context)
    : BaseRepository<Accommodation>(context), IAccommodationRepository
{
    public async Task<ICollection<Accommodation>> GetAllAsync(Guid destinationId, CancellationToken cancellationToken)
    {
        return await DbSet.Where(a => a.DestinationId == destinationId).ToListAsync(cancellationToken);
    }
}