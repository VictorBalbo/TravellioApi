using Microsoft.AspNetCore.Mvc;
using TravellioApi.Models;
using TravellioApi.Services;

namespace TravellioApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlacesController(IPlaceService placeService) : ControllerBase
{
    // GET: api/places/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Place>> GetPlace(string id, CancellationToken cancellationToken)
    {
        var place = await placeService.GetPlaceDetails(id, cancellationToken);
        if (place == null)
        {
            return NotFound();
        }

        return Ok(place);
    }
}