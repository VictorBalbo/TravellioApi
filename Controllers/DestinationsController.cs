using Microsoft.AspNetCore.Mvc;
using TravellioApi.Models;
using TravellioApi.Repositories;

namespace TravellioApi.Controllers;

[ApiController]
[Route("api/trips/{tripId:guid}/[controller]")]
public class DestinationsController(IDestinationRepository destinationRepository) : ControllerBase
{
    // GET: api/Trip/1/Destinations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Destination>>> GetDestinations(Guid tripId, CancellationToken cancellationToken)
    {
        var destinations = await destinationRepository.GetAllAsync(tripId, cancellationToken); 
        if (destinations.Count == 0)
            return NotFound();

        return Ok(destinations);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Destination>> GetDestination(Guid tripId, Guid id, CancellationToken cancellationToken)
    {
        var destination = await destinationRepository.GetByIdAsync(tripId, id, cancellationToken); 
        if (destination != null)
            return NotFound();

        return Ok(destination);
    }
}