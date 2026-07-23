using Travellio.Domain.DTOs;

namespace Travellio.Api.Services.PlaceProviders;

public interface IPlaceProvider
{
    public string ProviderName { get; }

    /// <summary>Lower values are tried first when falling back between providers.</summary>
    public int Priority { get; }

    Task<Place?> GetPlaceDetailsAsync(string placeId, CancellationToken cancellationToken);

    Task<IEnumerable<AutoComplete>?> GetAutoCompleteAsync(
        string text,
        string sessionToken,
        double lat,
        double lng,
        double radius,
        string language,
        string locationType,
        CancellationToken cancellationToken);

    Task<IEnumerable<AutoComplete>?> GetAutoCompleteAsync(
        string text,
        string sessionToken,
        double lat,
        double lng,
        double radius,
        string language,
        CancellationToken cancellationToken) =>
        GetAutoCompleteAsync(text, sessionToken, lat, lng, radius, language, "", cancellationToken);
}