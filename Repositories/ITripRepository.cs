using TravellioApi.Models.Entities;

namespace TravellioApi.Repositories;

public interface ITripRepository : IRepository<Trip>
{
    public Task<IEnumerable<Trip>> GetAllAsync(CancellationToken cancellationToken);
}