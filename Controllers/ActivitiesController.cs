using Microsoft.AspNetCore.Mvc;
using TravellioApi.Models.DTOs;
using TravellioApi.Queries;
using TravellioApi.Services;

namespace TravellioApi.Controllers;

[ApiController]
[Route("Api/Trips/{tripId:guid}/Destinations/{destinationId:guid}/[controller]")]
public class ActivitiesController(IActivityService activityService, IActivityQuery activityQuery)
    : ControllerBase
{
    // GET: Api/Trips/{tripId}/Destinations/{destinationId}/Activities
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ActivityDto>>> GetActivities(Guid tripId, Guid destinationId,
        CancellationToken cancellationToken)
    {
        var activities = await activityQuery.GetAllAsync(destinationId, cancellationToken);
        if (activities.Count == 0)
            return NotFound();

        return Ok(activities);
    }

    // GET: Api/Trips/{tripId}/Destinations/{destinationId}/Activities/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ActivityDto>> GetActivity(Guid tripId, Guid destinationId, Guid id,
        CancellationToken cancellationToken)
    {
        var activity = await activityQuery.GetByIdAsync(destinationId, id, true, cancellationToken);
        if (activity == null)
            return NotFound();

        return Ok(activity);
    }

    // POST: Api/Trips/{tripId}/Destinations/{destinationId}/Activities
    [HttpPost]
    public async Task<ActionResult<ActivityDto>> PostActivity(Guid tripId, Guid destinationId, ActivityDto activity,
        CancellationToken cancellationToken)
    {
        var activityDto = await activityService.AddOrUpdateAsync(activity, destinationId, cancellationToken);
        return CreatedAtAction(nameof(GetActivity), new { tripId, destinationId, id = activityDto.Id }, activityDto);
    }

    // DELETE: Api/Trips/{tripId}/Destinations/{destinationId}/Activities/{id}
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteActivity(Guid tripId, Guid destinationId, Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await activityService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
