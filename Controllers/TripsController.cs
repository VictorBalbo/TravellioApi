using Microsoft.AspNetCore.Mvc;
using TravellioApi.Models;
using TravellioApi.Repositories;

namespace TravellioApi.Controllers;

//TODO: Add auth
[ApiController]
[Route("api/[controller]")]
public class TripsController(ITripRepository tripRepository) : ControllerBase
{
    // GET: api/Trips
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Trip>>> GetTrips(CancellationToken cancellationToken)
    {
        var trips = await tripRepository.GetAllAsync(cancellationToken);
        if (!trips.Any())
            return NotFound();

        return Ok(trips);
    }
    
    // GET: api/Trips/1
    [HttpGet("{tripId:guid}")]
    public async Task<ActionResult<Trip>> GetTrip(Guid tripId, CancellationToken cancellationToken)
    {
        var trip = await tripRepository.GetByIdAsync(tripId, cancellationToken);
        if (trip == null)
        {
            return NotFound();
        }
        
        return trip;
    }
    
    // POST: api/Trips
    [HttpPost]
    public async Task<ActionResult<Trip>> PostTrip(Trip trip, CancellationToken cancellationToken)
    {
        await tripRepository.AddOrUpdateAsync(trip, cancellationToken);
        return CreatedAtAction("GetTrip", new { tripId = trip.Id }, trip);
    }

    // DELETE: api/Trips/1
    [HttpDelete("{tripId:guid}")]
    public async Task<ActionResult> DeleteTrip(Guid tripId, CancellationToken cancellationToken)
    {
        var deleted = await tripRepository.DeleteAsync(tripId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}