using Travellio.Models;
using Travellio.Services.PlaceDetailsProvider;

namespace Travellio.Services
{
    public class PlaceService(IPlaceProvider internalProvider, IExternalPlaceProvider externalProvider) : IPlaceService
    {
        private readonly IPlaceProvider _internalProvider = internalProvider;
        private readonly IExternalPlaceProvider _externalProvider = externalProvider;

        public async Task<Place?> GetPlaceDetails(string placeId)
        {
            Place? place;
            place = await _internalProvider.GetPlaceDetailsAsync(placeId);
            if (place != null)
            {
                return place;
            }

            place = await _externalProvider.GetPlaceDetailsAsync(placeId);
            if (place != null)
            {
                await _internalProvider.SetPlaceDetailsAsync(place);
                return place;
            }

            return null;
        }
    }
}
