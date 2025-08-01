using Travellio.Models;

namespace Travellio.Repository;

public interface IActivityRepository : IRepository<Activity>
{
    Task<IEnumerable<Activity>> GetAllAsync(Guid tripId, Guid destinationId);
    Task<Activity?> GetByIdAsync(Guid tripId, Guid destinationId, Guid id);
}
