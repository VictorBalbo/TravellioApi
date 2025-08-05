using Travellio.Models;

namespace Travellio.Services.PlaceDetailsProvider
{
    public interface IPlaceProvider
    {
        Task<Place?> GetPlaceDetailsAsync(string placeId);
        Task<bool> SetPlaceDetailsAsync(Place place);
    }
}
