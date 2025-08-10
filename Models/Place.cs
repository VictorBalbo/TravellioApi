using System.Text.Json.Serialization;

namespace Travellio.Models;

public class Place
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required Coordinates Coordinates { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }
    public IEnumerable<string>? Categories { get; set; }
    public string? Vicinity { get; set; }
    public double? Rating { get; set; }
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
    public required IEnumerable<string> WeekdayText { get; set; }

    public required IEnumerable<Periods> Periods { get; set; }
}

public class Periods
{
    public required PeriodHours Open { get; set; }
    public required PeriodHours Close { get; set; }
}
public class PeriodHours
{
    public required short Day { get; set; }
    public required string Time { get; set; }
}
