using System.ComponentModel.DataAnnotations;

namespace TravellioApi.Models.Entities;

public class Price
{
    public required decimal Value { get; set; }
    [MaxLength(3)]
    public required string Currency { get; set; }
}