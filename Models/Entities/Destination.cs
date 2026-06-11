namespace TravellioApi.Models.Entities;

public class Destination : IBaseEntity
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required Coordinates Coordinates { get; set; }
    public required string PlaceId { get; set; }
    public required DateOnly StartDate { get; set; }
    public required DateOnly EndDate { get; set; }
    public string? Notes { get; set; }
    public ICollection<Accommodation>? Accommodations { get; set; }
    public ICollection<Activity>? Activities { get; set; }

    // Foreign Key
    public required Guid TripId { get; set; }
}