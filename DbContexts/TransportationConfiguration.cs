using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravellioApi.Models;

namespace TravellioApi.DbContexts;

public class TransportationConfiguration : IEntityTypeConfiguration<Transportation>
{
    public void Configure(EntityTypeBuilder<Transportation> builder)
    {
        builder.HasKey(p => p.Id);
        builder.OwnsOne(p => p.Price, PriceConfiguration.Configure);

        // Transportation → ArrivalDestination (N:1)
        builder
            .HasOne(tp => tp.Arrival)
            .WithMany()
            .HasForeignKey(tp => tp.ArrivalDestinationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Transportation → DepartureDestination (N:1)
        builder
            .HasOne(tp => tp.Departure)
            .WithMany()
            .HasForeignKey(tp => tp.DepartureDestinationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Transportation → Leg (1:N)
        builder
            .HasMany(t => t.Legs)
            .WithOne()
            .HasForeignKey(ts => ts.TransportationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}