using Travellio.Domain.Entities;

namespace Travellio.Api.Repositories;

public interface IDestinationRepository : IRepository<Destination>
{
    Task<ICollection<Destination>> GetAllAsync(Guid tripId, CancellationToken cancellationToken);
    Task<Destination?> GetByIdAsync(Guid tripId, Guid id, CancellationToken cancellationToken);
}