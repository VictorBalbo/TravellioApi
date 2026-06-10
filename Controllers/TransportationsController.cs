using Microsoft.AspNetCore.Mvc;
using TravellioApi.Models;
using TravellioApi.Repositories;

namespace TravellioApi.Controllers;

[ApiController]
[Route("Api/Trips/{tripId:guid}/[controller]")]
public class TransportationsController(ITransportationRepository transportationRepository) : ControllerBase
{
    // GET: api/Trips/1/Transportations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transportation>>> GetTransportations(Guid tripId,
        CancellationToken cancellationToken)
    {
        var transportations = await transportationRepository.GetAllAsync(tripId, cancellationToken);
        if (transportations.Count == 0)
            return NotFound();

        return Ok(transportations);
    }

    // GET: api/Trips/{tripId}/Transportations/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Transportation>> GetTransportations(Guid tripId, Guid id,
        CancellationToken cancellationToken)
    {
        var transportation = await transportationRepository.GetByIdAsync(tripId, id, cancellationToken);
        if (transportation == null)
            return NotFound();

        return Ok(transportation);
    }

    // POST: api/Trips/{tripId}/Transportations
    [HttpPost]
    public async Task<ActionResult<Transportation>> PostTransportations(Guid tripId, Transportation transportation,
        CancellationToken cancellationToken)
    {
        transportation.TripId = tripId;
        await transportationRepository.AddOrUpdateAsync(transportation, cancellationToken);
        return CreatedAtAction(nameof(GetTransportations), new { tripId, id = transportation.Id }, transportation);
    }

    // DELETE: api/Trips/{tripId}/Transportations/{id}
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteTransportations(Guid tripId, Guid id, CancellationToken cancellationToken)
    {
        var deleted = await transportationRepository.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}