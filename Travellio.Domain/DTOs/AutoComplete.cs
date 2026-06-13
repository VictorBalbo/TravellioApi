namespace Travellio.Domain.DTOs;

public class AutoComplete
{
    public string? PlaceId { get; set; }
    public string? MainText { get; set; }
    public string? SecondaryText { get; set; }
    public AutoCompleteMatchedSubstrings? MainTextMatchedSubstrings { get; set; }
}

public class AutoCompleteMatchedSubstrings
{
    public int Offset { get; set; }
    public int Length { get; set; }
}