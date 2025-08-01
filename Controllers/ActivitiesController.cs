using Microsoft.AspNetCore.Mvc;
using Travellio.Models;
using Travellio.Repository;

namespace Travellio.Controllers;

[ApiController]
[Route("trips/{tripId}/destinations/{destinationId}/[controller]")]
public class ActivitiesController(IActivityRepository activityRepository) : ControllerBase
{
    private readonly IActivityRepository _activityRepository = activityRepository;

    // GET: Trip/1/Destinations/5/activities
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Activity>>> GetActivities(Guid tripId, Guid destinationId)
    {
        var activities = await _activityRepository.GetAllAsync(tripId, destinationId);
        if (activities == null || !activities.Any())
            return NotFound();

        return Ok(activities);
    }

    // GET: Trip/1/Destinations/5/activities/2
    [HttpGet("{id}")]
    public async Task<ActionResult<Activity>> GetActivity(Guid tripId, Guid destinationId, Guid id)
    {
        var activity = await _activityRepository.GetByIdAsync(tripId, destinationId, id);
        if (activity == null)
        {
            return NotFound();
        }

        return Ok(activity);
    }

    // POST: Trip/1/Destinations/5/activities
    [HttpPost]
    public async Task<ActionResult<Activity>> PostActivity(Activity activity)
    {
        await _activityRepository.AddOrUpdateAsync(activity);
        var routeValues = new
        {
            tripId = activity.Destination.TripId,
            destinationId = activity.DestinationId,
            id = activity.Id
        };
        return CreatedAtAction("GetActivity", routeValues, activity);
    }

    // DELETE: Trip/1/Destinations/5/activities/2
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteActivity(Guid id)
    {
        await _activityRepository.DeleteAsync(id);
        return Ok();
    }
}
