namespace TravellioApi.Models;

public class Transportation : IModel
{
    public Guid Id { get; set; }
    public Price? Price { get; set; }
    public required ICollection<TransportationSegment> Segments { get; set; }

    // Foreign Keys
    public Guid? OriginId { get; set; }
    public Guid? DestinationId { get; set; }
    public Guid TripId { get; set; }

    // Navigation properties
    public Destination? Origin { get; set; }
    public Destination? Destination { get; set; }
}