using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Travellio.Models;

public class Transportation : IModel
{
    [Key]
    public Guid Id { get; set; }

    public Price? Price { get; set; }

    // Foreign Keys
    [Required]
    public required Guid OriginId { get; set; }

    [Required]
    public required Guid DestinationId { get; set; }

    public Guid TripId { get; set; }

    // Navigation properties
    public required IEnumerable<TransportationSegment> Segments { get; set; }

    [JsonIgnore]
    [ValidateNever]
    public Destination Origin { get; set; } = null!;

    [JsonIgnore]
    [ValidateNever]
    public Destination Destination { get; set; } = null!;

    [JsonIgnore]
    [ValidateNever]
    public Trip Trip { get; set; } = null!;
}
public class TransportationSegment : IModel
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(40)]
    [Column(TypeName = "VARCHAR(40)")]
    public required string OriginTerminalPlaceId { get; set; }

    [Required]
    [MaxLength(40)]
    [Column(TypeName = "VARCHAR(40)")]
    public required string DestinationTerminalPlaceId { get; set; }

    [Required]
    [MaxLength(10)]
    [Column(TypeName = "VARCHAR(10)")]
    public required string Path { get; set; }

    [Required]
    public TransportTypes Type { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public Price? Price { get; set; }

    [MaxLength(30)]
    public string? Company { get; set; }

    [MaxLength(10)]
    public string? TransportIdentification { get; set; }

    [MaxLength(10)]
    public string? Reservation { get; set; }

    [MaxLength(10)]
    public string? Seat { get; set; }

    // Foreign Key
    public Guid TransportationId { get; set; }

    // Navigation properties
    [JsonIgnore]
    [ValidateNever]
    public Transportation Transportation { get; set; } = null!;

    // DTO Objects
    [NotMapped]
    public Place? OriginTerminal { get; set; }
    [NotMapped]
    public Place? DestinationTerminal { get; set; }
}
public enum TransportTypes
{
    Bus,
    Car,
    Plane,
    Ship,
    Train,
}
