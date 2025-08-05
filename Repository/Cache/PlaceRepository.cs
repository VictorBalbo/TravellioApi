using System.Text.Json;
using StackExchange.Redis;
using Travellio.Models;

namespace Travellio.Repository.Cache
{
    public class PlaceRepository(IConnectionMultiplexer redis) : IPlaceRepository
    {
        private readonly IDatabase _db = redis.GetDatabase();

        public async Task<Place?> GetPlaceDetailsAsync(string placeId)
        {
            var placeKey = $"place:{placeId}";
            var value = await _db.StringGetAsync(placeKey);

            if (value.IsNullOrEmpty)
                return null;

            return JsonSerializer.Deserialize<Place>(value.ToString());
        }

        public async Task<bool> AddPlaceDetailsAsync(Place place)
        {
            var placeKey = $"place:{place.Id}";
            var placeJson = JsonSerializer.Serialize(place);
            return await _db.StringSetAsync(placeKey, placeJson);
        }
    }
}
