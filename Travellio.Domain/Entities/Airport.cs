namespace Travellio.Domain.Entities;

public class Airport
{
    public required string IataCode { get; set; }
    public required string Name { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
    public required string CountryCode { get; set; }
    public string? City { get; set; }
}