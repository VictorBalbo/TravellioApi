using Travellio.Models;

namespace Travellio.Repository.Cache;

public interface IPlaceRepository
{
    Task<Place?> GetPlaceDetailsAsync(string placeId);
    Task<bool> AddPlaceDetailsAsync(Place place);
}
