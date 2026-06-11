namespace TravellioApi.Models.Entities;

public class Activity : IBaseEntity
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string PlaceId { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public bool? TicketRequired { get; set; } = false;
    public bool? TicketPurchased { get; set; } = false;
    public Price? Price { get; set; }
    public string? Website { get; set; }
    public string? Notes { get; set; }

    // Foreign Key
    public required Guid DestinationId { get; set; }
}