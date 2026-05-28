using TravellioApi.Models;

namespace TravellioApi.Services.PlaceProviders;

public class NullCachedPlaceProvider : ICachedPlaceProvider
{
    public Task<Place?> GetPlaceDetailsAsync(string placeId, CancellationToken cancellationToken) =>
        Task.FromResult<Place?>(null);

    public Task<bool> SetPlaceDetailsAsync(Place place, CancellationToken cancellationToken) =>
        Task.FromResult(false);
}