using Microsoft.AspNetCore.Mvc;
using Travellio.Models;
using Travellio.Repository;

namespace Travellio.Controllers;

[ApiController]
[Route("[controller]")]
public class TripsController(ITripRepository tripRepository) : ControllerBase
{
    private readonly ITripRepository _tripRepository = tripRepository;

    // GET: api/Trips
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Trip>>> GetTrips()
    {
        var trips = await _tripRepository.GetAllAsync(); 
        if (trips == null || !trips.Any())
            return NotFound();

        return Ok(trips);
    }

    // GET: api/Trips/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Trip>> GetTrip(Guid id)
    {
        var trip = await _tripRepository.GetByIdAsync(id);
        if (trip == null)
        {
            return NotFound();
        }

        return trip;
    }

    // POST: api/Trips
    [HttpPost]
    public async Task<ActionResult<Trip>> PostTrip(Trip trip)
    {
        await _tripRepository.AddOrUpdateAsync(trip);
        return CreatedAtAction("GetTrip", new { id = trip.Id }, trip);
    }

    // DELETE: api/Trips/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTrip(Guid id)
    {
        await _tripRepository.DeleteAsync(id);
        return Ok();
    }
}
