using TravellioApi.Models.DTOs;

namespace TravellioApi.Services;

public interface IActivityService
{
    Task<ActivityDto> AddOrUpdateAsync(ActivityDto dto, Guid destinationId, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
