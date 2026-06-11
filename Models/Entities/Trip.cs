namespace TravellioApi.Models.Entities;

public class Trip : IBaseEntity
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required DateOnly StartDate { get; set; }
    public required DateOnly EndDate { get; set; }
    public string? HomePlaceId { get; set; }

    // Navigation properties
    public ICollection<Destination>? Destinations { get; set; }
    public ICollection<Transportation>? Transportations { get; set; }
}