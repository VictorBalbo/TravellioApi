using Travellio.Domain.DTOs;

namespace Travellio.Api.Queries;

public interface IActivityQuery
{
    Task<ICollection<ActivityDto>> GetAllAsync(Guid destinationId, CancellationToken cancellationToken);
    Task<ActivityDto?> GetByIdAsync(Guid destinationId, Guid id, bool enrichPlaces, CancellationToken cancellationToken);
}
