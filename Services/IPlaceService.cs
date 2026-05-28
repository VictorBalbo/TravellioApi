using TravellioApi.Models;

namespace TravellioApi.Services;

public interface IPlaceService
{
    Task<Place?> GetPlaceDetails(string placeId, CancellationToken cancellationToken);
}