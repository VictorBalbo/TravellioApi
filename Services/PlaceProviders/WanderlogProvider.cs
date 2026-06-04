using TravellioApi.Models;
using TravellioApi.Models.Wanderlog;

namespace TravellioApi.Services.PlaceProviders;

public class WanderlogProvider(HttpClient httpClient, IConfiguration configuration) : IPlaceProvider
{
    public async Task<Place?> GetPlaceDetailsAsync(string placeId, CancellationToken cancellationToken)
    {
        var placeDetailsUrl = configuration["WanderlogPlaceDetailsUrl"]?.Replace("{placeId}", placeId);
        var placeMetadataUrl = configuration["WanderlogMetadataUrl"]?.Replace("{placeId}", placeId);

        if (placeDetailsUrl == null || placeMetadataUrl == null)
        {
            throw new InvalidOperationException("Wanderlog Urls not configured");
        }

        var detailsRequest = new HttpRequestMessage(HttpMethod.Get, placeDetailsUrl);
        var detailsResponseTask = httpClient.SendAsync(detailsRequest, cancellationToken);

        var metadataRequest = new HttpRequestMessage(HttpMethod.Get, placeMetadataUrl);
        var metadataResponseTask = httpClient.SendAsync(metadataRequest, cancellationToken);

        await Task.WhenAll(detailsResponseTask, metadataResponseTask);

        var detailsResponse = await detailsResponseTask;
        var metadataResponse = await metadataResponseTask;

        WanderlogPlaceDetails? placeDetails = null;
        if (detailsResponse.IsSuccessStatusCode)
        {
            var response =
                await detailsResponse.Content.ReadFromJsonAsync<WanderlogResponse<WanderlogPlaceDetails>>(
                    cancellationToken);
            placeDetails = response?.Data;
        }

        WanderlogPlaceMetadata? placeMetadata = null;
        if (metadataResponse.IsSuccessStatusCode)
        {
            var response =
                await metadataResponse.Content
                    .ReadFromJsonAsync<WanderlogResponse<IEnumerable<WanderlogPlaceMetadata>>>(cancellationToken);
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
            Vicinity = placeDetails?.AddressComponents?.FirstOrDefault(a => a.Types.Contains("locality", StringComparer.OrdinalIgnoreCase))?.ShortName ??
                       placeDetails?.Vicinity,
            Rating = placeDetails?.Rating ?? placeMetadata?.Rating,
            PhoneNumber = placeDetails?.InternationalPhoneNumber ?? placeMetadata?.InternationalPhoneNumber,
            Website = placeDetails?.Website ?? placeMetadata?.Website,
            BusinessStatus = placeDetails?.BusinessStatus,
            MapsUrl = placeDetails?.Url,
            Images = placeMetadata?.ImageKeys,
            OpeningHours = placeDetails?.OpeningHours,
        };
    }

    public Task<bool> AddPlaceDetailsAsync(Place place, CancellationToken cancellationToken)
    {
        return Task.FromResult(false);
    }
}