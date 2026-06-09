using TravellioApi.Models;
using TravellioApi.Models.DTOs;
using TravellioApi.Services.PlaceProviders;

namespace TravellioApi.Services;

public class PlaceService(IPlaceProvider externalProvider, ICachedPlaceProvider cachedProvider) : IPlaceService
{
    public async Task<Place?> GetPlaceDetails(string placeId, CancellationToken cancellationToken)
    {
        var place = await cachedProvider.GetPlaceDetailsAsync(placeId, cancellationToken);
        if (place != null)
        {
            return place;
        }

        place = await externalProvider.GetPlaceDetailsAsync(placeId, cancellationToken);
        if (place != null && !string.IsNullOrEmpty(place.Name))
        {
            await cachedProvider.SetPlaceDetailsAsync(place, cancellationToken);
            return place;
        }

        return null;
    }

    public async Task<IEnumerable<AutoComplete>?> GetAutoComplete(string text, string sessionToken, double lat,
        double lng, double radius,
        string language, CancellationToken cancellationToken)
    {
        var autoComplete =
            await cachedProvider.GetAutoCompleteAsync(text, sessionToken, lat, lng, radius, language,
                cancellationToken);
        if (autoComplete != null)
        {
            return autoComplete;
        }

        autoComplete = await externalProvider.GetAutoCompleteAsync(text, sessionToken, lat, lng, radius, language,
            cancellationToken);
        var autoCompletes = autoComplete?.ToArray();
        if (autoCompletes != null && autoCompletes.Length != 0)
        {
            await cachedProvider.SetAutoCompleteAsync(autoCompletes, text, language, cancellationToken);
            return autoCompletes;
        }

        return null;
    }
}