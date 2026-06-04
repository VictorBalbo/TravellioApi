namespace TravellioApi.Models;

public class Transportation : IBaseEntity
{
    public Guid Id { get; set; }
    public Price? Price { get; set; }
    public required ICollection<Leg> Legs { get; set; } = [];

    // Foreign Keys
    public Guid? ArrivalDestinationId { get; set; }
    public Guid? DepartureDestinationId { get; set; }
    public Guid TripId { get; set; }

    // Navigation properties
    public Destination? Arrival { get; set; }
    public Destination? Departure { get; set; }

    // Computed properties
    public DateTime? DepartureTime => Legs.OrderBy(l => l.DepartureTime).FirstOrDefault()?.DepartureTime;
    public DateTime? ArrivalTime => Legs.OrderBy(l => l.DepartureTime).LastOrDefault()?.ArrivalTime;
}