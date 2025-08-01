using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Travellio.Models;

[Owned]
public class Price
{
    [Precision(9, 2)]
    public decimal Value { get; set; }

    [MaxLength(3)]
    [Column(TypeName = "VARCHAR(3)")]
    public required string Currency { get; set; }
}
