using Travellio.Domain.Entities;

namespace Travellio.Api.Repositories;

public interface ITransportationRepository : IRepository<Transportation>
{
    Task<ICollection<Transportation>> GetAllAsync(Guid tripId, CancellationToken cancellationToken);
    Task<Transportation?> GetByIdAsync(Guid tripId, Guid id, CancellationToken cancellationToken);
}