using Travellio.Models;

namespace Travellio.Repository;

public interface IDestinationRepository : IRepository<Destination>
{
    Task<IEnumerable<Destination>> GetAllAsync(Guid tripId);
    Task<Destination?> GetByIdAsync(Guid tripId, Guid id);
}