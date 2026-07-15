namespace Travellio.Api.Models.GooglePlaces;

public class GoogleAutoCompleteRequest
{
    public required string Input { get; set; }
    public IEnumerable<string>? IncludedPrimaryTypes { get; set; }
    public string? SessionToken { get; set; }
    public string? LanguageCode { get; set; }
    public GoogleLocationBias? LocationBias { get; set; }
}

public class GoogleLocationBias
{
    public GoogleCircle? Circle { get; set; }
}

public class GoogleCircle
{
    public GoogleLatLng? Center { get; set; }
    public double? Radius { get; set; }
}

public class GoogleLatLng
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}