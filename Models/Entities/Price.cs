namespace TravellioApi.Models.Entities;

public class Price
{
    public required decimal Value { get; set; }
    public required string Currency { get; set; }
}