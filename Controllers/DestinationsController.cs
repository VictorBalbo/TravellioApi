using Microsoft.AspNetCore.Mvc;
using TravellioApi.Models.Entities;
using TravellioApi.Repositories;

namespace TravellioApi.Controllers;

[ApiController]
[Route("Api/Trips/{tripId:guid}/[controller]")]
public class DestinationsController(IDestinationRepository destinationRepository) : ControllerBase
{
    // GET: api/Trips/1/Destinations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Destination>>> GetDestinations(Guid tripId,
        CancellationToken cancellationToken)
    {
        var destinations = await destinationRepository.GetAllAsync(tripId, cancellationToken);
        if (destinations.Count == 0)
            return NotFound();

        return Ok(destinations);
    }

    // GET: api/Trips/{tripId}/Destinations/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Destination>> GetDestination(Guid tripId, Guid id,
        CancellationToken cancellationToken)
    {
        var destination = await destinationRepository.GetByIdAsync(tripId, id, cancellationToken);
        if (destination == null)
            return NotFound();

        return Ok(destination);
    }

    // POST: api/Trips/{tripId}/destinations
    [HttpPost]
    public async Task<ActionResult<Destination>> PostDestination(Guid tripId, Destination destination,
        CancellationToken cancellationToken)
    {
        destination.TripId = tripId;
        await destinationRepository.AddOrUpdateAsync(destination, cancellationToken);
        return CreatedAtAction(nameof(GetDestination), new { tripId, id = destination.Id }, destination);
    }

    // DELETE: api/Trips/{tripId}/destinations/{id}
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteDestination(Guid tripId, Guid id, CancellationToken cancellationToken)
    {
        var deleted = await destinationRepository.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}