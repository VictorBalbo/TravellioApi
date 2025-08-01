using Microsoft.AspNetCore.Mvc;
using Travellio.Models;
using Travellio.Repository;

namespace Travellio.Controllers;

[ApiController]
[Route("trips/{tripId}/destinations/{destinationId}/[controller]")]
public class AccommodationsController(IAccommodationRepository accommodationRepository) : ControllerBase
{
    private readonly IAccommodationRepository _accommodationRepository = accommodationRepository;

    // GET: Trip/1/Destinations/5/accommodations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Accommodation>>> GetAccommodations(Guid tripId, Guid destinationId)
    {
        var accommodations = await _accommodationRepository.GetAllAsync(tripId, destinationId);
        if (accommodations == null || !accommodations.Any())
            return NotFound();

        return Ok(accommodations);
    }

    // GET: Trip/1/Destinations/5/accommodations/2
    [HttpGet("{id}")]
    public async Task<ActionResult<Accommodation>> GetAccommodation(Guid tripId, Guid destinationId, Guid id)
    {
        var accommodation = await _accommodationRepository.GetByIdAsync(tripId, destinationId, id);
        if (accommodation == null)
        {
            return NotFound();
        }

        return Ok(accommodation);
    }

    // POST: Trip/1/Destinations/5/accommodations
    [HttpPost]
    public async Task<ActionResult<Accommodation>> PostAccommodation(Accommodation activity)
    {
        await _accommodationRepository.AddOrUpdateAsync(activity);
        var routeValues = new
        {
            tripId = activity.Destination.TripId,
            destinationId = activity.DestinationId,
            id = activity.Id
        };
        return CreatedAtAction("GetAccommodation", routeValues, activity);
    }

    // DELETE: Trip/1/Destinations/5/accommodations/2
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAccommodation(Guid id)
    {
        await _accommodationRepository.DeleteAsync(id);
        return Ok();
    }
}
