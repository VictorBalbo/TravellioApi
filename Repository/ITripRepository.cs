using Travellio.Models;

namespace Travellio.Repository;

public interface ITripRepository : IRepository<Trip>
{
    Task<IEnumerable<Trip>> GetAllAsync();
    Task<Trip?> GetByIdAsync(Guid id);
}
