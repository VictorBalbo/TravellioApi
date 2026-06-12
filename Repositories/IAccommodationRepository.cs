using TravellioApi.Models.Entities;

namespace TravellioApi.Repositories;

public interface IAccommodationRepository : IRepository<Accommodation>
{
    Task<ICollection<Accommodation>> GetAllAsync(Guid destinationId, CancellationToken cancellationToken);
}
