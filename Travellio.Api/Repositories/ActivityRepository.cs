using Microsoft.EntityFrameworkCore;
using Travellio.Domain.Entities;
using Travellio.Infrastructure.DbContexts;

namespace Travellio.Api.Repositories;

public class ActivityRepository(AppDbContext context) : BaseRepository<Activity>(context), IActivityRepository
{
    public async Task<ICollection<Activity>> GetAllAsync(Guid destinationId, CancellationToken cancellationToken)
    {
        return await DbSet.Where(a => a.DestinationId == destinationId).ToListAsync(cancellationToken);
    }
}