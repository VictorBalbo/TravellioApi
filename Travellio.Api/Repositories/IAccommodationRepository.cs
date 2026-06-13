using Travellio.Domain.Entities;

namespace Travellio.Api.Repositories;

public interface IAccommodationRepository : IRepository<Accommodation>
{
    Task<ICollection<Accommodation>> GetAllAsync(Guid destinationId, CancellationToken cancellationToken);
}
