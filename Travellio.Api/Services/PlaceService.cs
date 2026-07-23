using Serilog;
using Travellio.Api.Services.PlaceProviders;
using Travellio.Domain.DTOs;

namespace Travellio.Api.Services;

public class PlaceService(
    ICachedPlaceProvider cachedProvider,
    IEnumerable<IPlaceProvider> externalProviders,
    IDiagnosticContext diagnosticContext,
    ILogger<PlaceService> logger) : IPlaceService
{
    private readonly IPlaceProvider[] _providers =
        new IPlaceProvider[] { cachedProvider }
            .Concat(externalProviders)
            .OrderBy(p => p.Priority)
            .ToArray();

    public async Task<Place?> GetPlaceDetails(string placeId, CancellationToken cancellationToken)
    {
        foreach (var provider in _providers)
        {
            try
            {
                var place = await provider.GetPlaceDetailsAsync(placeId, cancellationToken);
                if (place == null) continue;

                diagnosticContext.Set("ProviderHit", provider.ProviderName);
                if (provider.ProviderName != cachedProvider.ProviderName)
                {
                    await cachedProvider.SetPlaceDetailsAsync(place, cancellationToken);
                }

                return place;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "{Provider} GetPlaceDetails failed for {PlaceId}, falling back to next provider",
                    provider.ProviderName,
                    placeId);
            }
        }

        return null;
    }

    public async Task<IEnumerable<AutoComplete>?> GetAutoComplete(string text, string sessionToken, double lat,
        double lng, double radius, string language, string locationType, CancellationToken cancellationToken)
    {
        foreach (var provider in _providers)
        {
            try
            {
                IEnumerable<AutoComplete>? autoComplete;
                if (string.IsNullOrEmpty(locationType))
                {
                    autoComplete = await provider.GetAutoCompleteAsync(text, sessionToken, lat, lng, radius,
                        language, cancellationToken);
                }
                else
                {
                    autoComplete = await provider.GetAutoCompleteAsync(text, sessionToken, lat, lng, radius,
                        language, locationType, cancellationToken);
                }

                if (autoComplete == null) continue;

                var autoCompleteArray = autoComplete.ToArray();
                if (provider.ProviderName != cachedProvider.ProviderName)
                {
                    await cachedProvider.SetAutoCompleteAsync(autoCompleteArray, text, language, cancellationToken);
                }

                diagnosticContext.Set("ProviderHit", provider.ProviderName);
                return autoCompleteArray;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "{Provider} GetAutoComplete failed for '{Text}', falling back to next provider",
                    provider.ProviderName,
                    text);
            }
        }

        return null;
    }
}