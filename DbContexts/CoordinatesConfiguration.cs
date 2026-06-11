using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravellioApi.Models.Entities;

namespace TravellioApi.DbContexts;

public static class CoordinatesConfiguration
{
    public static void Configure<TOwner>(OwnedNavigationBuilder<TOwner, Coordinates> builder) where TOwner : class
    {
        builder.Property(p => p.Lat)
            .HasPrecision(8, 6)
            .IsRequired();
        builder.Property(p => p.Lng)
            .HasPrecision(9, 6)
            .IsRequired();
    }
}