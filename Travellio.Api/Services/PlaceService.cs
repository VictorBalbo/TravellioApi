using Serilog;
using Travellio.Api.Services.PlaceProviders;
using Travellio.Domain.DTOs;

namespace Travellio.Api.Services;

public class PlaceService(
    IPlaceProvider externalProvider,
    ICachedPlaceProvider cachedProvider,
    IDiagnosticContext diagnosticContext) : IPlaceService
{
    public async Task<Place?> GetPlaceDetails(string placeId, CancellationToken cancellationToken)
    {
        var place = await cachedProvider.GetPlaceDetailsAsync(placeId, cancellationToken);
        if (place != null)
        {
            diagnosticContext.Set("CacheResult", "Hit");
            return place;
        }

        diagnosticContext.Set("CacheResult", "Miss");


        place = await externalProvider.GetPlaceDetailsAsync(placeId, cancellationToken);
        if (place != null && !string.IsNullOrEmpty(place.Name))
        {
            await cachedProvider.SetPlaceDetailsAsync(place, cancellationToken);
            return place;
        }

        return null;
    }

    public async Task<IEnumerable<AutoComplete>?> GetAutoComplete(string text, string sessionToken, double lat,
        double lng, double radius, string language, string locationType, CancellationToken cancellationToken)
    {
        var autoComplete =
            await cachedProvider.GetAutoCompleteAsync(text, sessionToken, lat, lng, radius, language, locationType,
                cancellationToken);
        if (autoComplete != null)
        {
            diagnosticContext.Set("CacheResult", "Hit");
            return autoComplete;
        }

        diagnosticContext.Set("CacheResult", "Miss");


        autoComplete = await externalProvider.GetAutoCompleteAsync(text, sessionToken, lat, lng, radius, language,
            locationType, cancellationToken);
        var autoCompletes = autoComplete?.ToArray();
        if (autoCompletes != null && autoCompletes.Length != 0)
        {
            await cachedProvider.SetAutoCompleteAsync(autoCompletes, text, language, cancellationToken);
            return autoCompletes;
        }

        return null;
    }
}