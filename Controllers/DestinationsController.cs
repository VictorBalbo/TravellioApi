using Microsoft.AspNetCore.Mvc;
using Travellio.Models;
using Travellio.Repository;

namespace Travellio.Controllers;

[ApiController]
[Route("trips/{tripId}/[controller]")]
public class DestinationsController(IDestinationRepository destinationRepository) : ControllerBase
{
    private readonly IDestinationRepository _destinationRepository = destinationRepository;

    // GET: Trip/1/Destinations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Destination>>> GetDestinations(Guid tripId)
    {
        var destinations = await _destinationRepository.GetAllAsync(tripId); 
        if (destinations == null || !destinations.Any())
            return NotFound();

        return Ok(destinations);
    }

    // GET: Trip/1/Destinations/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Destination>> GetDestination(Guid tripId, Guid id)
    {
        var destination = await _destinationRepository.GetByIdAsync(tripId, id);
        if (destination == null)
        {
            return NotFound();
        }

        return Ok(destination);
    }

    // POST: Trip/1/Destinations
    [HttpPost]
    public async Task<ActionResult<Destination>> PostDestination(Destination destination)
    {
        await _destinationRepository.AddOrUpdateAsync(destination);
        return CreatedAtAction("GetDestination", new { tripId = destination.TripId, id = destination.Id }, destination);
    }

    // DELETE: Trip/1/Destinations/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDestination(Guid id)
    {
        await _destinationRepository.DeleteAsync(id);
        return Ok();
    }
}
