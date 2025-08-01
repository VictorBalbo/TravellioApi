
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Travellio.Models;

public class Destination : IModel
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public required DateTime StartDate { get; set; }

    [Required]
    public required DateTime EndDate { get; set; }

    [Required]
    [MaxLength(40)]
    [Column(TypeName = "VARCHAR(40)")]
    public required string PlaceId { get; set; }

    [MaxLength]
    public string? Notes { get; set; }

    // Foreign Key
    public Guid TripId { get; set; }

    // Navigation properties
    public IEnumerable<Accommodation>? Accommodations { get; set; }

    public IEnumerable<Activity>? Activities { get; set; }

    [JsonIgnore]
    [ValidateNever]
    public Trip Trip { get; set; } = null!;

    // DTO properties
    [NotMapped]
    public Place? Place { get; set; }
}
