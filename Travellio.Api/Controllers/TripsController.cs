using Microsoft.AspNetCore.Mvc;
using Travellio.Api.Queries;
using Travellio.Api.Services;
using Travellio.Domain.DTOs;

namespace Travellio.Api.Controllers;

//TODO: Add auth
[ApiController]
[Route("Api/[controller]")]
public class TripsController(ITripService tripService, ITripQuery tripQuery) : ControllerBase
{
    // GET: Api/Trips
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TripDto>>> GetTrips(CancellationToken cancellationToken)
    {
        var trips = await tripQuery.GetAllAsync(cancellationToken);
        if (trips.Count == 0)
            return NotFound();

        return Ok(trips);
    }

    // GET: Api/Trips/1
    [HttpGet("{tripId:guid}")]
    public async Task<ActionResult<TripDto>> GetTrip(Guid tripId, CancellationToken cancellationToken)
    {
        var trip = await tripQuery.GetByIdAsync(tripId, false, cancellationToken);
        if (trip == null)
            return NotFound();

        return Ok(trip);
    }

    // POST: Api/Trips
    [HttpPost]
    public async Task<ActionResult<TripDto>> PostTrip(TripDto trip, CancellationToken cancellationToken)
    {
        var tripDto = await tripService.AddOrUpdateAsync(trip, cancellationToken);
        return CreatedAtAction(nameof(GetTrip), new { tripId = tripDto.Id }, tripDto);
    }

    // DELETE: Api/Trips/1
    [HttpDelete("{tripId:guid}")]
    public async Task<ActionResult> DeleteTrip(Guid tripId, CancellationToken cancellationToken)
    {
        var deleted = await tripService.DeleteAsync(tripId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}