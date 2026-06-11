using Microsoft.AspNetCore.Mvc;
using TravellioApi.Models.DTOs;
using TravellioApi.Queries;
using TravellioApi.Services;

namespace TravellioApi.Controllers;

[ApiController]
[Route("Api/Trips/{tripId:guid}/[controller]")]
public class DestinationsController(IDestinationService destinationService, IDestinationQuery destinationQuery)
    : ControllerBase
{
    // GET: api/Trips/1/Destinations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DestinationDto>>> GetDestinations(Guid tripId,
        CancellationToken cancellationToken)
    {
        var destinations = await destinationQuery.GetAllAsync(tripId, cancellationToken);
        if (destinations.Count == 0)
            return NotFound();

        return Ok(destinations);
    }

    // GET: api/Trips/{tripId}/Destinations/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DestinationDto>> GetDestination(Guid tripId, Guid id,
        CancellationToken cancellationToken)
    {
        var destination = await destinationQuery.GetByIdAsync(tripId, id, true, cancellationToken);
        if (destination == null)
            return NotFound();

        return Ok(destination);
    }

    // POST: api/Trips/{tripId}/destinations
    [HttpPost]
    public async Task<ActionResult<DestinationDto>> PostDestination(Guid tripId, DestinationDto destination,
        CancellationToken cancellationToken)
    {
        var destinationDto = await destinationService.AddOrUpdateAsync(destination, tripId, cancellationToken);
        return CreatedAtAction(nameof(GetDestination), new { tripId, id = destination.Id }, destinationDto);
    }

    // DELETE: api/Trips/{tripId}/destinations/{id}
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteDestination(Guid tripId, Guid id, CancellationToken cancellationToken)
    {
        var deleted = await destinationService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}