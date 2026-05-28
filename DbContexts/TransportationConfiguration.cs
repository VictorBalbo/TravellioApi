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

        // Transportation → Origin (N:1)
        builder
            .HasOne(tp => tp.Origin)
            .WithMany()
            .HasForeignKey(tp => tp.OriginId)
            .OnDelete(DeleteBehavior.Restrict);

        // Transportation → Destination (N:1)
        builder
            .HasOne(tp => tp.Destination)
            .WithMany()
            .HasForeignKey(tp => tp.DestinationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Transportation → TransportationSegments (1:N)
        builder
            .HasMany(t => t.Segments)
            .WithOne()
            .HasForeignKey(ts => ts.TransportationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}