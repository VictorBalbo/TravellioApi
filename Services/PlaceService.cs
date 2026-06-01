using TravellioApi.Models;
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
}