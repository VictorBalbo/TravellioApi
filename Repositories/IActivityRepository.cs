using TravellioApi.Models.Entities;

namespace TravellioApi.Repositories;

public interface IActivityRepository : IRepository<Activity>
{
    Task<ICollection<Activity>> GetAllAsync(Guid destinationId, CancellationToken cancellationToken);
}
