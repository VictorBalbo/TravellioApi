using System.ComponentModel.DataAnnotations;

namespace TravellioApi.Models;

public class TransportationSegment : IModel
{
    public Guid Id { get; set; }
    public required string OriginTerminalPlaceId { get; set; }
    public required string DestinationTerminalPlaceId { get; set; }
    // public required string Path { get; set; }
    public TransportationTypes Type { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Price? Price { get; set; }
    public string? Company { get; set; }
    public string? TransportIdentification { get; set; }
    public string? Reservation { get; set; }
    public string? Seat { get; set; }

    // Foreign Key
    public Guid TransportationId { get; set; }

    // DTO Objects
    public Place? OriginTerminal { get; set; }
    public Place? DestinationTerminal { get; set; }
}