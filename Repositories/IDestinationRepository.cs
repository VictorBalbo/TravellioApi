using TravellioApi.Models.Entities;

namespace TravellioApi.Repositories;

public interface IDestinationRepository : IRepository<Destination>
{
    Task<ICollection<Destination>> GetAllAsync(Guid tripId, CancellationToken cancellationToken);
    Task<Destination?> GetByIdAsync(Guid tripId, Guid id, CancellationToken cancellationToken);
}