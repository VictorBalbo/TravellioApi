using System.Text.Json.Serialization;

namespace Travellio.Api.Models.Wanderlog;

public class WanderlogAutoCompleteResponse
{
    [JsonPropertyName("place_id")]
    public string? PlaceId { get; set; }
    public string? Description { get; set; }
    public IEnumerable<string>? Types { get; set; }
    [JsonPropertyName("structured_formatting")]
    public WanderlogStructuredFormattingResponse? StructuredFormatting { get; set; }
    [JsonPropertyName("matched_substrings")]
    public IEnumerable<WanderlogMatchedSubstringsResponse>? MatchedSubstrings { get; set; }

    public string? Type { get; set; }
    public string? Input { get; set; }
    public bool? CanSeeOnMap { get; set; }
}

public class WanderlogStructuredFormattingResponse
{
    [JsonPropertyName("main_text")]
    public string? MainText { get; set; }
    [JsonPropertyName("secondary_text")]
    public string? SecondaryText { get; set; }
    [JsonPropertyName("main_text_matched_substrings")]
    public IEnumerable<WanderlogMatchedSubstringsResponse>? MainTextMatchedSubstrings { get; set; }
}

public class WanderlogMatchedSubstringsResponse
{
    public int Offset { get; set; }
    public int Length { get; set; }
}