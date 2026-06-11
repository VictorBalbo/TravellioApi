using Microsoft.AspNetCore.Mvc;
using TravellioApi.Models.DTOs;
using TravellioApi.Services;

namespace TravellioApi.Controllers;

[ApiController]
[Route("Api/Trips/{tripId:guid}/[controller]")]
public class TransportationsController(ITransportationService transportationService) : ControllerBase
{
    // GET: api/Trips/1/Transportations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransportationDto>>> GetTransportations(Guid tripId,
        CancellationToken cancellationToken)
    {
        var transportations = await transportationService.GetAllAsync(tripId, cancellationToken);
        if (transportations.Count == 0)
            return NotFound();

        return Ok(transportations);
    }

    // GET: api/Trips/{tripId}/Transportations/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TransportationDto>> GetTransportation(Guid tripId, Guid id,
        CancellationToken cancellationToken)
    {
        var transportation = await transportationService.GetByIdAsync(tripId, id, cancellationToken);
        if (transportation == null)
            return NotFound();

        return Ok(transportation);
    }

    // POST: api/Trips/{tripId}/Transportations
    [HttpPost]
    public async Task<ActionResult<TransportationDto>> PostTransportation(Guid tripId,
        TransportationDto transportation, CancellationToken cancellationToken)
    {
        var transportationDto = await transportationService.AddOrUpdateAsync(transportation, tripId, cancellationToken);
        return CreatedAtAction(nameof(GetTransportation), new { tripId, id = transportation.Id },
            transportationDto);
    }

    // DELETE: api/Trips/{tripId}/Transportations/{id}
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteTransportation(Guid tripId, Guid id, CancellationToken cancellationToken)
    {
        var deleted = await transportationService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}