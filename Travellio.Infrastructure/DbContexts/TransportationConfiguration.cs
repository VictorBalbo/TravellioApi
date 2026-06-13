using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Travellio.Domain.Entities;

namespace Travellio.Infrastructure.DbContexts;

public class TransportationConfiguration : BaseEntityConfiguration<Transportation>
{
    public override void Configure(EntityTypeBuilder<Transportation> builder)
    {
        base.Configure(builder);

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