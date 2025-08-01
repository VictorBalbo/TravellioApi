using System.Text.Json.Serialization;

namespace Travellio.Models;

public class Place
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required Coordinates Coordinates { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }
    public IEnumerable<string>? Categories { get; set; }
    public string? Vicinity { get; set; }
    public int? Rating { get; set; }
    public string? Website { get; set; }
    public string? PhoneNumber { get; set; }
    public IEnumerable<string>? Images { get; set; }
    public string? BusinessStatus { get; set; }
    public string? MapsUrl { get; set; }
    public OpeningHours? OpeningHours { get; set; }
}

public class OpeningHours
{
    [JsonPropertyName("weekday_text")]
    public required string WeekdayText { get; set; }

    public required Periods Periods { get; set; }
}

public class Periods
{
    public required string Open { get; set; }
    public required string Close { get; set; }
}
public class PeriodHours
{
    public required short Day { get; set; }
    public required string Time { get; set; }
}
