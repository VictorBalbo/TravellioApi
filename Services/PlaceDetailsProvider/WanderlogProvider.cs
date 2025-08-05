using Travellio.Models;
using Travellio.Models.Wanderlog;

namespace Travellio.Services.PlaceDetailsProvider
{
    public class WanderlogProvider(HttpClient httpClient) : IExternalPlaceProvider
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<Place?> GetPlaceDetailsAsync(string placeId)
        {
            var placeDetailsUrl = $"https://wanderlog.com/api/placesAPI/getPlaceDetails/v2?placeId={placeId}&language=en-US";
            var placeMetadataUrl = $"https://wanderlog.com/api/places/metadata?placeIds={placeId}&getDetails=true";

            var detailsRequest = new HttpRequestMessage(HttpMethod.Get, placeDetailsUrl);
            var detailsResponseTask = _httpClient.SendAsync(detailsRequest);

            var metadataRequest = new HttpRequestMessage(HttpMethod.Get, placeMetadataUrl);
            var metadataResponseTask = _httpClient.SendAsync(metadataRequest);

            await Task.WhenAll(detailsResponseTask, metadataResponseTask);

            var detailsResponse = await detailsResponseTask;
            var metadataResponse = await metadataResponseTask;

            WanderlogPlaceDetails? placeDetails = null;
            if (detailsResponse.IsSuccessStatusCode)
            {
                var response = await detailsResponse.Content.ReadFromJsonAsync<WanderlogResponse<WanderlogPlaceDetails>>();
                placeDetails = response?.Data;
            }
            WanderlogPlaceMetadata? placeMetadata = null;
            if (metadataResponse.IsSuccessStatusCode)
            {
                var response = await metadataResponse.Content.ReadFromJsonAsync<WanderlogResponse<IEnumerable<WanderlogPlaceMetadata>>>();
                placeMetadata = response?.Data?.FirstOrDefault();
            }


            return new Place
            {
                Id = placeId,
                Name = placeDetails?.Name ?? placeMetadata?.Name ?? "",
                Coordinates = new Coordinates
                {
                    Lat = placeDetails?.Geometry?.Location.Lat ?? 0,
                    Lng = placeDetails?.Geometry?.Location.Lng ?? 0
                },
                Address = placeDetails?.FormattedAddress ?? placeMetadata?.Address,
                Description = placeMetadata?.GeneratedDescription ?? placeMetadata?.Description,
                Categories = placeMetadata?.Categories,
                Vicinity = placeDetails?.Vicinity,
                Rating = placeDetails?.Rating ?? placeMetadata?.Rating,
                PhoneNumber = placeDetails?.InternationalPhoneNumber ?? placeMetadata?.InternationalPhoneNumber,
                Website = placeDetails?.Website ?? placeMetadata?.Website,
                BusinessStatus = placeDetails?.BusinessStatus,
                MapsUrl = placeDetails?.Url,
                Images = placeMetadata?.ImageKeys,
                OpeningHours = placeDetails?.OpeningHours,
            };
        }
    }
}
