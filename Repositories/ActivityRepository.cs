using Microsoft.EntityFrameworkCore;
using TravellioApi.DbContexts;
using TravellioApi.Models.Entities;

namespace TravellioApi.Repositories;

public class ActivityRepository(AppDbContext context)
    : BaseRepository<Activity>(context), IActivityRepository
{
    public async Task<ICollection<Activity>> GetAllAsync(Guid destinationId, CancellationToken cancellationToken)
    {
        return await DbSet.Where(a => a.DestinationId == destinationId).ToListAsync(cancellationToken);
    }
}
