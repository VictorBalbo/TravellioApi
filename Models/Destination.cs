namespace TravellioApi.Models;

public class Destination : IModel
{
    public Guid Id { get; set; }
    public required string PlaceId { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public string? Notes { get; set; }
    public ICollection<Accommodation>? Accommodations { get; set; }
    public ICollection<Activity>? Activities { get; set; }

    // Foreign Key
    public Guid TripId { get; set; }

    // DTO properties
    public Place? Place { get; set; }
}