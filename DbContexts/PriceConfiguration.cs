using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravellioApi.Models;

namespace TravellioApi.DbContexts;

public static class PriceConfiguration
{
    public static void Configure<TOwner>(OwnedNavigationBuilder<TOwner,Price> builder) where TOwner : class
    {
        builder.Property(p => p.Value)
            .IsRequired()
            .HasPrecision(9, 2);
        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(3);
    }
}