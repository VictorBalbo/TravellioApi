namespace TravellioApi.Models.Entities;

public class Accommodation : IBaseEntity
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string PlaceId { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public string? ImageUrl { get; set; }
    public string? Website { get; set; }
    public string? Notes { get; set; }
    public Price? Price { get; set; }

    // Foreign Key
    public required Guid DestinationId { get; set; }
}