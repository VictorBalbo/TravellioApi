using Travellio.Domain.Entities;

namespace Travellio.Api.Repositories;

public interface IActivityRepository : IRepository<Activity>
{
    Task<ICollection<Activity>> GetAllAsync(Guid destinationId, CancellationToken cancellationToken);
}
