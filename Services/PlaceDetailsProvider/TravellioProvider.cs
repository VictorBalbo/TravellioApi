using Travellio.Models;
using Travellio.Repository.Cache;

namespace Travellio.Services.PlaceDetailsProvider
{
    public class TravellioProvider(IPlaceRepository placeRepository) : IPlaceProvider
    {
        private readonly IPlaceRepository _placeRepository = placeRepository;

        public async Task<Place?> GetPlaceDetailsAsync(string placeId) 
            => await _placeRepository.GetPlaceDetailsAsync(placeId);

        public async Task<bool> SetPlaceDetailsAsync(Place place)
            => await _placeRepository.AddPlaceDetailsAsync(place);

    }
}
