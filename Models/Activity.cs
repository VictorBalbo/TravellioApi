namespace TravellioApi.Models;

public class Activity : IModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string PlaceId { get; set; }
    public DateTime? DateTime { get; set; }
    public Price? Price { get; set; }
    public string? Website { get; set; }
    public string? Notes { get; set; }

    // Foreign Key
    public Guid? DestinationId { get; set; }

    // DTO properties
    public Place? Place { get; set; }
}