using System.Text.Json.Serialization;

namespace Travellio.Models.Wanderlog;

public record WanderlogPlaceDetails
{
    [JsonPropertyName("place_id")]
    public required string PlaceId { get; set; }

    public required string Name { get; set; }

    public required string? Url { get; set; }
    public string? Vicinity { get; set; }

    [JsonPropertyName("formatted_address")]
    public string? FormattedAddress { get; set; }

    [JsonPropertyName("address_components")]
    public IEnumerable<AddressComponent>? AddressComponents { get; set; }

    public Geometry? Geometry { get; set; }

    public double? Rating { get; set; }

    [JsonPropertyName("user_ratings_total")]
    public int? UserRatingsTotal { get; set; }

    public string? Website { get; set; }

    [JsonPropertyName("international_phone_number")]
    public string? InternationalPhoneNumber { get; set; }

    [JsonPropertyName("opening_hours")]
    public OpeningHours? OpeningHours { get; set; }

    public IEnumerable<string>? Types { get; set; }

    [JsonPropertyName("business_status")]
    public string? BusinessStatus { get; set; }
}

public record AddressComponent
{
    [JsonPropertyName("long_name")]
    public required string LongName { get; init; }

    [JsonPropertyName("short_name")]
    public required string ShortName { get; init; }
    public required IEnumerable<string> Types { get; init; }
}

public class Geometry
{
    public required Coordinates Location { get; set; }
}