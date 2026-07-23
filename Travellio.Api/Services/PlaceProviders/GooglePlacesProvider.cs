using System.Text.Json;
using Travellio.Api.Models.GooglePlaces;
using Travellio.Domain.DTOs;
using Travellio.Domain.Entities;

namespace Travellio.Api.Services.PlaceProviders;

public class GooglePlacesProvider(
    HttpClient httpClient,
    IConfiguration configuration,
    ILogger<GooglePlacesProvider> logger) : IPlaceProvider
{
    private const string BaseUrl = "https://places.googleapis.com/v1";

    private const string DetailsFieldMask =
        "id,displayName,formattedAddress,location,rating,userRatingCount,websiteUri,internationalPhoneNumber," +
        "businessStatus,googleMapsUri,regularOpeningHours,editorialSummary,types,addressComponents";

    private static readonly JsonSerializerOptions SerializerOptions = new()
        { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private const int GoogleMaxRadius = 50_000;

    public string ProviderName { get; } = nameof(GooglePlacesProvider);
    public int Priority { get; } = 2;

    public async Task<Place?> GetPlaceDetailsAsync(string placeId, CancellationToken cancellationToken)
    {
        var apiKey = GetApiKey();

        var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/places/{placeId}");
        request.Headers.Add("X-Goog-Api-Key", apiKey);
        request.Headers.Add("X-Goog-FieldMask", DetailsFieldMask);

        var response = await httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogWarning("Google Places details request failed for {PlaceId} with status {StatusCode}: {Body}",
                placeId, response.StatusCode, body);
            return null;
        }

        var placeDetails =
            await response.Content.ReadFromJsonAsync<GooglePlaceDetails>(SerializerOptions, cancellationToken);
        if (placeDetails is null)
        {
            return null;
        }

        return new Place
        {
            Id = placeId,
            Name = placeDetails.DisplayName?.Text ?? "",
            Coordinates = new Coordinates
            {
                Lat = (decimal)(placeDetails.Location?.Latitude ?? 0),
                Lng = (decimal)(placeDetails.Location?.Longitude ?? 0)
            },
            Address = placeDetails.FormattedAddress,
            Description = placeDetails.EditorialSummary?.Text,
            Categories = placeDetails.Types,
            Vicinity = placeDetails.AddressComponents
                ?.FirstOrDefault(a => a.Types.Contains("locality", StringComparer.OrdinalIgnoreCase))
                ?.ShortText,
            Rating = placeDetails.Rating,
            PhoneNumber = placeDetails.InternationalPhoneNumber,
            Website = placeDetails.WebsiteUri,
            BusinessStatus = placeDetails.BusinessStatus,
            MapsUrl = placeDetails.GoogleMapsUri,
            OpeningHours = MapOpeningHours(placeDetails.RegularOpeningHours),
        };
    }

    public async Task<IEnumerable<AutoComplete>?> GetAutoCompleteAsync(string text, string sessionToken, double lat,
        double lng, double radius, string language, string locationType, CancellationToken cancellationToken)
    {
        var apiKey = GetApiKey();
        var locationTypes = locationType.Split(',').Select(l => l.Trim()).ToArray();
        var autoCompleteRequest = new GoogleAutoCompleteRequest
        {
            Input = text,
            SessionToken = sessionToken,
            IncludedPrimaryTypes = locationTypes,
            LanguageCode = language,
            LocationBias = new GoogleLocationBias
            {
                Circle = new GoogleCircle
                {
                    Center = new GoogleLatLng { Latitude = lat, Longitude = lng },
                    Radius = Math.Min(radius, GoogleMaxRadius)
                }
            }
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/places:autocomplete")
        {
            Content = JsonContent.Create(autoCompleteRequest, options: SerializerOptions)
        };
        httpRequest.Headers.Add("X-Goog-Api-Key", apiKey);

        var response = await httpClient.SendAsync(httpRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            logger.LogWarning(
                "Google Places autocomplete request failed for '{Text}' with status {StatusCode}: {Body}", text,
                response.StatusCode, body);
            return null;
        }

        var autoCompleteResponse =
            await response.Content.ReadFromJsonAsync<GoogleAutoCompleteResponse>(SerializerOptions,
                cancellationToken);
        var predictions = autoCompleteResponse?.Suggestions
            ?.Select(s => s.PlacePrediction)
            .Where(p => !string.IsNullOrEmpty(p?.PlaceId));

        return predictions?.Select(p => new AutoComplete
        {
            PlaceId = p!.PlaceId,
            MainText = p.StructuredFormat?.MainText?.Text,
            SecondaryText = p.StructuredFormat?.SecondaryText?.Text,
            MainTextMatchedSubstrings = MapMatchedSubstring(p.StructuredFormat?.MainText?.Matches?.FirstOrDefault())
        }) ?? [];
    }

    private string GetApiKey()
    {
        var apiKey = configuration["GooglePlacesApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            logger.LogWarning("Google Places api key not configured");
            throw new InvalidOperationException("Google Places api key not configured");
        }

        return apiKey;
    }

    private static OpeningHours? MapOpeningHours(GoogleOpeningHours? openingHours)
    {
        if (openingHours is null)
        {
            return null;
        }

        return new OpeningHours
        {
            WeekdayText = openingHours.WeekdayDescriptions ?? [],
            Periods = openingHours.Periods?.Select(p => new Periods
            {
                Open = MapPoint(p.Open),
                Close = MapPoint(p.Close)
            }) ?? []
        };
    }

    private static PeriodHours? MapPoint(GooglePoint? point)
    {
        if (point is null)
        {
            return null;
        }

        return new PeriodHours
        {
            Day = (short)point.Day,
            Time = $"{point.Hour:D2}{point.Minute:D2}"
        };
    }

    private static AutoCompleteMatchedSubstrings MapMatchedSubstring(GoogleStringRange? range)
    {
        var start = range?.StartOffset ?? 0;
        var end = range?.EndOffset ?? 0;

        return new AutoCompleteMatchedSubstrings
        {
            Offset = start,
            Length = end - start
        };
    }
}