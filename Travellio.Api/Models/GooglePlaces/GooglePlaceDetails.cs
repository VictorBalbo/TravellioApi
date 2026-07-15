namespace Travellio.Api.Models.GooglePlaces;

public record GooglePlaceDetails
{
    public string? Id { get; set; }
    public GoogleLocalizedText? DisplayName { get; set; }
    public string? FormattedAddress { get; set; }
    public GoogleLocation? Location { get; set; }
    public double? Rating { get; set; }
    public int? UserRatingCount { get; set; }
    public string? WebsiteUri { get; set; }
    public string? InternationalPhoneNumber { get; set; }
    public string? BusinessStatus { get; set; }
    public string? GoogleMapsUri { get; set; }
    public GoogleOpeningHours? RegularOpeningHours { get; set; }
    public GoogleLocalizedText? EditorialSummary { get; set; }
    public IEnumerable<string>? Types { get; set; }
    public IEnumerable<GoogleAddressComponent>? AddressComponents { get; set; }
}

public class GoogleLocalizedText
{
    public string? Text { get; set; }
    public string? LanguageCode { get; set; }
}

public class GoogleLocation
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class GoogleOpeningHours
{
    public IEnumerable<string>? WeekdayDescriptions { get; set; }
    public IEnumerable<GooglePeriod>? Periods { get; set; }
}

public class GooglePeriod
{
    public GooglePoint? Open { get; set; }
    public GooglePoint? Close { get; set; }
}

public class GooglePoint
{
    public int Day { get; set; }
    public int Hour { get; set; }
    public int Minute { get; set; }
}

public class GoogleAddressComponent
{
    public required string LongText { get; init; }
    public required string ShortText { get; init; }
    public required IEnumerable<string> Types { get; init; }
}
