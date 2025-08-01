using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Travellio.Models;

public class Activity : IModel
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(40)]
    [Column(TypeName = "VARCHAR(40)")]
    public required string PlaceId { get; set; }

    public DateTime? DateTime { get; set; }

    public Price? Price { get; set; }

    [MaxLength(255)]
    public string? Website { get; set; }

    [MaxLength]
    public string? Notes { get; set; }

    // Foreign Key
    public Guid? DestinationId { get; set; }

    // Navigation property
    [JsonIgnore]
    [ValidateNever]
    public Destination Destination { get; set; } = null!;

    // DTO properties
    [NotMapped]
    public Place? Place { get; set; }
}