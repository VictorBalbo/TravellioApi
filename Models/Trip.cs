namespace TravellioApi.Models;

public class Trip : IModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }

    // Navigation properties
    public ICollection<Destination>? Destinations { get; set; }
    public ICollection<Transportation>? Transportations { get; set; }
}