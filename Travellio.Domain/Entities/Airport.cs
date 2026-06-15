namespace Travellio.Domain.Entities;

public class Airport
{
    public required string IataCode { get; set; }
    public required string Name { get; set; }
    public decimal Lat { get; set; }
    public decimal Lng { get; set; }
    public required string CountryCode { get; set; }
    public string? City { get; set; }
}