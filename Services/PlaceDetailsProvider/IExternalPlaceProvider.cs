using Travellio.Models;

namespace Travellio.Services.PlaceDetailsProvider
{
    public interface IExternalPlaceProvider
    {
        Task<Place?> GetPlaceDetailsAsync(string placeId);
    }
}
