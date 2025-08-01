using Travellio.Models;

namespace Travellio.Repository;

public interface IAccommodationRepository : IRepository<Accommodation>
{
    Task<IEnumerable<Accommodation>> GetAllAsync(Guid tripId, Guid destinationId);
    Task<Accommodation?> GetByIdAsync(Guid tripId, Guid destinationId, Guid id);
}

