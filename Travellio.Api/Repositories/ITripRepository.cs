using Travellio.Domain.Entities;

namespace Travellio.Api.Repositories;

public interface ITripRepository : IRepository<Trip>
{
    public Task<ICollection<Trip>> GetAllAsync(CancellationToken cancellationToken);
}