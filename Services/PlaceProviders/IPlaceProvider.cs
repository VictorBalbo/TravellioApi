using TravellioApi.Models;

namespace TravellioApi.Services.PlaceProviders;

public interface IPlaceProvider
{
    Task<Place?> GetPlaceDetailsAsync(string placeId, CancellationToken cancellationToken);
}