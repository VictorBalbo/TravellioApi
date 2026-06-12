using TravellioApi.Models.DTOs;
using TravellioApi.Repositories;

namespace TravellioApi.Services;

public class ActivityService(IActivityRepository activityRepository) : IActivityService
{
    public async Task<ActivityDto> AddOrUpdateAsync(ActivityDto dto, Guid destinationId, CancellationToken cancellationToken)
    {
        var activity = dto.ToEntity(destinationId);
        await activityRepository.AddOrUpdateAsync(activity, cancellationToken);
        return activity.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await activityRepository.DeleteAsync(id, cancellationToken);
    }
}
