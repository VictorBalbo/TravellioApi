using Travellio.Domain.DTOs;

namespace Travellio.Api.Services.PlaceProviders;

public interface IPlaceProvider
{
    Task<Place?> GetPlaceDetailsAsync(string placeId, CancellationToken cancellationToken);

    Task<IEnumerable<AutoComplete>?> GetAutoCompleteAsync(
        string text,
        string sessionToken,
        double lat,
        double lng,
        double radius,
        string language, CancellationToken cancellationToken);
}