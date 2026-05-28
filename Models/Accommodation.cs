namespace TravellioApi.Models;

public class Accommodation : IModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string PlaceId { get; set; }
    public DateTime? Checkin { get; set; }
    public DateTime? Checkout { get; set; }
    public string? ImageUrl { get; set; }
    public string? Website { get; set; }
    public string? Notes { get; set; }
    public Price? Price { get; set; }

    // Foreign Key
    public Guid? DestinationId { get; set; }

    // DTO properties
    public Place? Place { get; set; }
}