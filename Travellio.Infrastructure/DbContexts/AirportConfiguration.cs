using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Travellio.Domain.Entities;

namespace Travellio.Infrastructure.DbContexts;

public class AirportConfiguration : IEntityTypeConfiguration<Airport>
{
    public void Configure(EntityTypeBuilder<Airport> builder)
    {
        builder.HasKey(a => a.IataCode);
        builder.Property(a => a.IataCode).HasMaxLength(3).ValueGeneratedNever();

        builder.Property(a => a.Name).HasMaxLength(100).IsRequired();
        builder.Property(a => a.CountryCode).HasMaxLength(2).IsRequired();
        builder.Property(a => a.City).HasMaxLength(50);
    }
}