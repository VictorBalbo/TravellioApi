using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Travellio.Models;

public class Accommodation : IModel
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [Required]
    [MaxLength(40)]
    [Column(TypeName = "VARCHAR(40)")]
    public required string PlaceId { get; set; }

    [MaxLength(255)]
    public string? ImageUrl {get; set; }

    public DateTime? Checkin {get; set;}

    public DateTime? Checkout {get; set;}

    [MaxLength(255)]
    public string? Website {get; set; }

    [MaxLength]
    public string? Notes {get; set; }

    public Price? Price { get; set; }

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
