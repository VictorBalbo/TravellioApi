using TravellioApi.Models;

namespace TravellioApi.Services.PlaceProviders;

public interface ICachedPlaceProvider : IPlaceProvider
{
    Task<bool> SetPlaceDetailsAsync(Place place, CancellationToken cancellationToken);
}