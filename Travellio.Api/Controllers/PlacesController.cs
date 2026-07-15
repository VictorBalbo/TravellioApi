using Microsoft.AspNetCore.Mvc;
using Travellio.Api.Services;
using Travellio.Domain.DTOs;

namespace Travellio.Api.Controllers;

[ApiController]
[Route("Api/[controller]")]
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

    // GET: api/places/autocomplete?input=paris&lat=48.8&lng=2.3&radius=5000&language=en
    [HttpGet("Autocomplete")]
    public async Task<ActionResult> GetAutoComplete(
        [FromQuery] string text,
        [FromQuery] double lat,
        [FromQuery] double lng,
        [FromQuery] double radius,
        [FromQuery] string language,
        [FromQuery] string? locationType,
        [FromQuery] string? sessionToken,
        CancellationToken cancellationToken)
    {
        var place = await placeService.GetAutoComplete(
            text,
            sessionToken ?? Guid.CreateVersion7().ToString(),
            lat,
            lng,
            radius,
            language,
            locationType ?? "",
            cancellationToken);

        return Ok(place);
    }
}