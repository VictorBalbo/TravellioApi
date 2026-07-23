using System.Text.Json;
using Travellio.Api.Models.Wanderlog;
using Travellio.Domain.DTOs;
using Travellio.Domain.Entities;

namespace Travellio.Api.Services.PlaceProviders;

public class WanderlogProvider(HttpClient httpClient, IConfiguration configuration, ILogger<WanderlogProvider> logger)
    : IPlaceProvider
{
    private static readonly JsonSerializerOptions SerializerOptions =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public string ProviderName { get; } = nameof(WanderlogProvider);
    public int Priority { get; } = 1;

    public async Task<Place?> GetPlaceDetailsAsync(string placeId, CancellationToken cancellationToken)
    {
        var placeDetailsUrl = configuration["WanderlogPlaceDetailsUrl"]?.Replace("{placeId}", placeId);
        var placeMetadataUrl = configuration["WanderlogMetadataUrl"]?.Replace("{placeId}", placeId);

        if (placeDetailsUrl == null || placeMetadataUrl == null)
        {
            logger.LogWarning("Wanderlog urls not configured");
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
        else
        {
            var body = await detailsResponse.Content.ReadAsStringAsync(cancellationToken);
            logger.LogWarning("Wanderlog PlaceDetails request failed for {PlaceId} with status {StatusCode}: {Body}",
                placeId, detailsResponse.StatusCode, body);
        }

        WanderlogPlaceMetadata? placeMetadata = null;
        if (metadataResponse.IsSuccessStatusCode)
        {
            var response =
                await metadataResponse.Content
                    .ReadFromJsonAsync<WanderlogResponse<IEnumerable<WanderlogPlaceMetadata>>>(cancellationToken);
            placeMetadata = response?.Data?.FirstOrDefault();
        }
        else
        {
            var body = await metadataResponse.Content.ReadAsStringAsync(cancellationToken);
            logger.LogWarning("Wanderlog PlaceMetadata request failed for {PlaceId} with status {StatusCode}: {Body}",
                placeId, metadataResponse.StatusCode, body);
        }

        if (placeDetails == null && placeMetadata == null)
        {
            return null;
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
            Vicinity = placeDetails?.AddressComponents
                           ?.FirstOrDefault(a => a.Types.Contains("locality", StringComparer.OrdinalIgnoreCase))
                           ?.ShortName ??
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

    public async Task<IEnumerable<AutoComplete>?> GetAutoCompleteAsync(string text, string sessionToken, double lat,
        double lng, double radius, string language, CancellationToken cancellationToken)
    {
        var autoCompleteUrl = configuration["WanderlogAutoCompleteUrl"];
        var location = new WanderlogAutoCompleteLocation() { Latitude = lat, Longitude = lng };
        var request = new WanderlogAutoCompleteRequest()
        {
            Input = text,
            SessionToken = sessionToken,
            Location = location,
            Radius = radius,
            Language = language
        };
        var serialized = Uri.EscapeDataString(JsonSerializer.Serialize(request, SerializerOptions));
        autoCompleteUrl = $"{autoCompleteUrl}?request={serialized}";

        var httpRequest = new HttpRequestMessage(HttpMethod.Get, autoCompleteUrl);
        var autoCompleteResponse = await httpClient.SendAsync(httpRequest, cancellationToken);

        if (!autoCompleteResponse.IsSuccessStatusCode)
        {
            var body = await autoCompleteResponse.Content.ReadAsStringAsync(cancellationToken);
            logger.LogWarning("Wanderlog AutoComplete request failed for '{Text}' with status {StatusCode}: {Body}",
                text, autoCompleteResponse.StatusCode, body);
            return null;
        }

        var response =
            await autoCompleteResponse.Content
                .ReadFromJsonAsync<WanderlogResponse<IEnumerable<WanderlogAutoCompleteResponse>>>(cancellationToken);
        var wanderlogAutoComplete = response?.Data?.Where(r => !string.IsNullOrEmpty(r.PlaceId));

        return wanderlogAutoComplete?.Select(r => new AutoComplete
        {
            PlaceId = r.PlaceId,
            MainText = r.StructuredFormatting?.MainText,
            SecondaryText = r.StructuredFormatting?.SecondaryText,
            MainTextMatchedSubstrings = new AutoCompleteMatchedSubstrings()
            {
                Length = r.StructuredFormatting?.MainTextMatchedSubstrings?.FirstOrDefault()?.Length ?? 0,
                Offset = r.StructuredFormatting?.MainTextMatchedSubstrings?.FirstOrDefault()?.Offset ?? 0,
            },
        }) ?? [];
    }

    /// <summary>
    /// Wanderlog does not support autocomplete requests with the locationType param.
    /// </summary>
    /// <returns>null</returns>
    public Task<IEnumerable<AutoComplete>?> GetAutoCompleteAsync(string text, string sessionToken, double lat,
        double lng, double radius, string locationType, string language,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<AutoComplete>?>(null);
    }
}