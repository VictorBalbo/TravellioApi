using Travellio.Domain.DTOs;

namespace Travellio.Api.Services.PlaceProviders;

public class FallbackPlaceProvider(
    WanderlogProvider primaryProvider,
    GooglePlacesProvider fallbackProvider,
    ILogger<FallbackPlaceProvider> logger) : IPlaceProvider
{
    public async Task<Place?> GetPlaceDetailsAsync(string placeId, CancellationToken cancellationToken)
    {
        try
        {
            var place = await primaryProvider.GetPlaceDetailsAsync(placeId, cancellationToken);
            if (place != null && !string.IsNullOrEmpty(place.Name))
            {
                return place;
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Wanderlog GetPlaceDetails failed for {PlaceId}, falling back to Google Places",
                placeId);
        }

        return await fallbackProvider.GetPlaceDetailsAsync(placeId, cancellationToken);
    }

    public async Task<IEnumerable<AutoComplete>?> GetAutoCompleteAsync(string text, string sessionToken, double lat,
        double lng, double radius, string language, string locationType, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(locationType))
            {
                var autoComplete = await primaryProvider.GetAutoCompleteAsync(text, sessionToken, lat, lng, radius,
                    language, locationType, cancellationToken);
                if (autoComplete != null)
                {
                    return autoComplete;
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Wanderlog GetAutoComplete failed for '{Text}', falling back to Google Places",
                text);
        }


        return await fallbackProvider.GetAutoCompleteAsync(text, sessionToken, lat, lng, radius, language, locationType,
            cancellationToken);
    }
}