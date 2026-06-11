using TravellioApi.Models.Entities;

namespace TravellioApi.Repositories;

public interface ITripRepository : IRepository<Trip>
{
    public Task<ICollection<Trip>> GetAllAsync(CancellationToken cancellationToken);
}