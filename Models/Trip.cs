using System.ComponentModel.DataAnnotations;

namespace Travellio.Models;

public class Trip : IModel
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }
    [Required]
    public required DateTime StartDate { get; set; }
    [Required]
    public required DateTime EndDate { get; set; }

    // Navigation properties
    public IEnumerable<Destination>? Destinations { get; set; }
    public IEnumerable<Transportation>? Transportations { get; set; }
}

