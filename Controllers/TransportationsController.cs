using Microsoft.AspNetCore.Mvc;
using TravellioApi.Models.DTOs;
using TravellioApi.Queries;
using TravellioApi.Services;

namespace TravellioApi.Controllers;

[ApiController]
[Route("Api/Trips/{tripId:guid}/[controller]")]
public class TransportationsController(
    ITransportationService transportationService,
    ITransportationQuery transportationQuery) : ControllerBase
{
    // GET: Api/Trips/1/Transportations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransportationDto>>> GetTransportations(Guid tripId,
        CancellationToken cancellationToken)
    {
        var transportations = await transportationQuery.GetAllAsync(tripId, cancellationToken);
        if (transportations.Count == 0)
            return NotFound();

        return Ok(transportations);
    }

    // GET: Api/Trips/{tripId}/Transportations/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TransportationDto>> GetTransportation(Guid tripId, Guid id,
        CancellationToken cancellationToken)
    {
        var transportation = await transportationQuery.GetByIdAsync(tripId, id, true, cancellationToken);
        if (transportation == null)
            return NotFound();

        return Ok(transportation);
    }

    // POST: Api/Trips/{tripId}/Transportations
    [HttpPost]
    public async Task<ActionResult<TransportationDto>> PostTransportation(Guid tripId,
        TransportationDto transportation, CancellationToken cancellationToken)
    {
        var transportationDto = await transportationService.AddOrUpdateAsync(transportation, tripId, cancellationToken);
        return CreatedAtAction(nameof(GetTransportation), new { tripId, id = transportationDto.Id },
            transportationDto);
    }

    // DELETE: Api/Trips/{tripId}/Transportations/{id}
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteTransportation(Guid tripId, Guid id, CancellationToken cancellationToken)
    {
        var deleted = await transportationService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}