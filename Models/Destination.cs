namespace TravellioApi.Models;

public class Destination : IBaseEntity
{
    public Guid Id { get; set; }
    public required string PlaceId { get; set; }
    public required DateOnly StartDate { get; set; }
    public required DateOnly EndDate { get; set; }
    public string? Notes { get; set; }
    public ICollection<Accommodation>? Accommodations { get; set; }
    public ICollection<Activity>? Activities { get; set; }

    // Foreign Key
    public Guid TripId { get; set; }

    // DTO properties
    public Place? Place { get; set; }
}