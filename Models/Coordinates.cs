using Microsoft.EntityFrameworkCore;

namespace Travellio.Models;

[Owned]
public class Coordinates
{
    [Precision(9, 6)]
    public decimal Lat { get; set; }
    [Precision(9, 6)]
    public decimal Lng { get; set; }
}
