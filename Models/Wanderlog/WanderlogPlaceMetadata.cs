namespace Travellio.Models.Wanderlog;

public record WanderlogPlaceMetadata
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public string? PlaceId { get; init; }
    public string? Description { get; init; }
    public string? GeneratedDescription { get; init; }
    public IEnumerable<string>? Categories { get; init; }
    public bool? HasDetails { get; init; }
    public string? Address { get; init; }
    public double? Rating { get; init; }
    public double? TripadvisorRating { get; init; }
    public string? Website { get; init; }
    public string? InternationalPhoneNumber { get; init; }
    public IEnumerable<string>? ImageKeys { get; init; }
    public bool? PermanentlyClosed { get; init; }
}
