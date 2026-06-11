using Microsoft.AspNetCore.Mvc;
using TravellioApi.Models.DTOs;
using TravellioApi.Queries;
using TravellioApi.Services;

namespace TravellioApi.Controllers;

//TODO: Add auth
[ApiController]
[Route("Api/[controller]")]
public class TripsController(ITripService tripService, ITripQuery tripQuery) : ControllerBase
{
    // GET: api/Trips
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TripDto>>> GetTrips(CancellationToken cancellationToken)
    {
        var trips = await tripQuery.GetAllAsync(cancellationToken);
        if (trips.Count == 0)
            return NotFound();

        return Ok(trips);
    }

    // GET: api/Trips/1
    [HttpGet("{tripId:guid}")]
    public async Task<ActionResult<TripDto>> GetTrip(Guid tripId, CancellationToken cancellationToken)
    {
        var trip = await tripQuery.GetByIdAsync(tripId, true, cancellationToken);
        if (trip == null)
            return NotFound();

        return Ok(trip);
    }

    // POST: api/Trips
    [HttpPost]
    public async Task<ActionResult<TripDto>> PostTrip(TripDto trip, CancellationToken cancellationToken)
    {
        var tripDto = await tripService.AddOrUpdateAsync(trip, cancellationToken);
        return CreatedAtAction(nameof(GetTrip), new { tripId = tripDto.Id }, tripDto);
    }

    // DELETE: api/Trips/1
    [HttpDelete("{tripId:guid}")]
    public async Task<ActionResult> DeleteTrip(Guid tripId, CancellationToken cancellationToken)
    {
        var deleted = await tripService.DeleteAsync(tripId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}