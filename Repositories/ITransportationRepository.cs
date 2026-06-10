using TravellioApi.Models.Entities;

namespace TravellioApi.Repositories;

public interface ITransportationRepository : IRepository<Transportation>
{
    Task<ICollection<Transportation>> GetAllAsync(Guid tripId, CancellationToken cancellationToken);
    Task<Transportation?> GetByIdAsync(Guid tripId, Guid id, CancellationToken cancellationToken);
}