using Microsoft.AspNetCore.Mvc;
using Travellio.Models;
using Travellio.Repository;

namespace Travellio.Controllers;

[ApiController]
[Route("trips/{tripId}/[controller]")]
public class TransportationsController(ITransportationRepository transportationRepository) : ControllerBase
{
    private readonly ITransportationRepository _transportationRepository = transportationRepository;

    // GET: Trip/1/Transportations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transportation>>> GetTransportations(Guid tripId)
    {
        var transportations = await _transportationRepository.GetAllAsync(tripId);
        if (transportations == null || !transportations.Any())
            return NotFound();

        return Ok(transportations);
    }

    // GET: Trip/1/Transportations/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Transportation>> GetTransportation(Guid tripId, Guid id)
    {
        var transportation = await _transportationRepository.GetByIdAsync(tripId, id);
        if (transportation == null)
        {
            return NotFound();
        }

        return Ok(transportation);
    }

    // POST: Trip/1/Transportations
    [HttpPost]
    public async Task<ActionResult<Transportation>> PostTransportation(Transportation transportation)
    {
        await _transportationRepository.AddOrUpdateAsync(transportation);
        return CreatedAtAction("GetTransportation", new { tripId = transportation.TripId, id = transportation.Id }, transportation);
    }

    // DELETE: Trip/1/Transportations/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDestination(Guid id)
    {
        await _transportationRepository.DeleteAsync(id);
        return Ok();
    }
}
