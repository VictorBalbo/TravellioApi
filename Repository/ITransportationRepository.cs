using Travellio.Models;

namespace Travellio.Repository;

public interface ITransportationRepository : IRepository<Transportation>
{
    Task<IEnumerable<Transportation>> GetAllAsync(Guid tripId);
    Task<Transportation?> GetByIdAsync(Guid tripId, Guid id);
}