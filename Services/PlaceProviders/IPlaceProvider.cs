using TravellioApi.Models.DTOs;
using TravellioApi.Models.Entities;

namespace TravellioApi.Services.PlaceProviders;

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