namespace Travellio.Domain.Entities;

public class Transportation : IBaseEntity
{
    public required Guid Id { get; set; }
    public Price? Price { get; set; }
    public required ICollection<Leg> Legs { get; set; } = [];

    // Foreign Keys
    public Guid? ArrivalDestinationId { get; set; }
    public Guid? DepartureDestinationId { get; set; }
    public Guid TripId { get; set; }

    // Navigation properties
    public Destination? Arrival { get; set; }
    public Destination? Departure { get; set; }
}