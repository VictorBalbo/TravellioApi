using Travellio.Models;

namespace Travellio.Services
{
    public interface IPlaceService
    {
        Task<Place?> GetPlaceDetails(string placeId);
    }
}
