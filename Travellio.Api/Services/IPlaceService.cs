using Travellio.Domain.DTOs;

namespace Travellio.Api.Services;

public interface IPlaceService
{
    Task<Place?> GetPlaceDetails(string placeId, CancellationToken cancellationToken);

    Task<IEnumerable<AutoComplete>?> GetAutoComplete(string text, string sessionToken, double lat,
        double lng, double radius,
        string language, CancellationToken cancellationToken);
}