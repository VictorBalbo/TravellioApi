using TravellioApi.Models.DTOs;

namespace TravellioApi.Queries;

public interface IActivityQuery
{
    Task<ICollection<ActivityDto>> GetAllAsync(Guid destinationId, CancellationToken cancellationToken);
    Task<ActivityDto?> GetByIdAsync(Guid destinationId, Guid id, bool enrichPlaces, CancellationToken cancellationToken);
}
