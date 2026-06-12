using Microsoft.AspNetCore.Mvc;
using TravellioApi.Models.DTOs;
using TravellioApi.Queries;
using TravellioApi.Services;

namespace TravellioApi.Controllers;

[ApiController]
[Route("Api/Trips/{tripId:guid}/Destinations/{destinationId:guid}/[controller]")]
public class AccommodationsController(
    IAccommodationService accommodationService,
    IAccommodationQuery accommodationQuery)
    : ControllerBase
{
    // GET: Api/Trips/{tripId}/Destinations/{destinationId}/Accommodations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccommodationDto>>> GetAccommodations(Guid tripId, Guid destinationId,
        CancellationToken cancellationToken)
    {
        var accommodations = await accommodationQuery.GetAllAsync(destinationId, cancellationToken);
        if (accommodations.Count == 0)
            return NotFound();

        return Ok(accommodations);
    }

    // GET: Api/Trips/{tripId}/Destinations/{destinationId}/Accommodations/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AccommodationDto>> GetAccommodation(Guid tripId, Guid destinationId, Guid id,
        CancellationToken cancellationToken)
    {
        var accommodation = await accommodationQuery.GetByIdAsync(destinationId, id, true, cancellationToken);
        if (accommodation == null)
            return NotFound();

        return Ok(accommodation);
    }

    // POST: Api/Trips/{tripId}/Destinations/{destinationId}/Accommodations
    [HttpPost]
    public async Task<ActionResult<AccommodationDto>> PostAccommodation(Guid tripId, Guid destinationId,
        AccommodationDto accommodation, CancellationToken cancellationToken)
    {
        var accommodationDto =
            await accommodationService.AddOrUpdateAsync(accommodation, destinationId, cancellationToken);
        return CreatedAtAction(nameof(GetAccommodation),
            new { tripId, destinationId, id = accommodationDto.Id }, accommodationDto);
    }

    // DELETE: Api/Trips/{tripId}/Destinations/{destinationId}/Accommodations/{id}
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAccommodation(Guid tripId, Guid destinationId, Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await accommodationService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}