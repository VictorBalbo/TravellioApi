namespace TravellioApi.Models;

public class Leg : IModel
{
    public Guid Id { get; set; }
    public required string DeparturePlaceId { get; set; }
    public required string ArrivalPlaceId { get; set; }
    public TransportationType Type { get; set; }
    public DateTime? DepartureTime { get; set; }
    public DateTime? ArrivalTime { get; set; }
    public Price? Price { get; set; }
    public string? Company { get; set; }
    public string? ServiceNumber { get; set; }
    public string? Reservation { get; set; }
    public string? Seat { get; set; }

    // Foreign Key
    public Guid TransportationId { get; set; }

    // DTO Objects
    public Place? DeparturePlace { get; set; }
    public Place? ArrivalPlace { get; set; }
}