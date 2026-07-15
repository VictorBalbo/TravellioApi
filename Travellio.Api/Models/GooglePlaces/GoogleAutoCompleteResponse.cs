namespace Travellio.Api.Models.GooglePlaces;

public class GoogleAutoCompleteResponse
{
    public IEnumerable<GoogleSuggestion>? Suggestions { get; set; }
}

public class GoogleSuggestion
{
    public GooglePlacePrediction? PlacePrediction { get; set; }
}

public class GooglePlacePrediction
{
    public string? Place { get; set; }
    public string? PlaceId { get; set; }
    public GoogleFormattableText? Text { get; set; }
    public GoogleStructuredFormat? StructuredFormat { get; set; }
}

public class GoogleFormattableText
{
    public string? Text { get; set; }
    public IEnumerable<GoogleStringRange>? Matches { get; set; }
}

public class GoogleStructuredFormat
{
    public GoogleFormattableText? MainText { get; set; }
    public GoogleFormattableText? SecondaryText { get; set; }
}

public class GoogleStringRange
{
    public int StartOffset { get; set; }
    public int EndOffset { get; set; }
}