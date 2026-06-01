using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravellioApi.Models;

namespace TravellioApi.DbContexts;

public class TripConfiguration : IEntityTypeConfiguration<Trip>
{
    public void Configure(EntityTypeBuilder<Trip> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(p => p.StartDate)
            .IsRequired();
        builder.Property(p => p.EndDate)
            .IsRequired();
        builder.Property(p => p.HomePlaceId)
            .HasMaxLength(Constants.PlaceIdSize);

        // Trip → Destinations (1:N)
        builder
            .HasMany(t => t.Destinations)
            .WithOne()
            .HasForeignKey(d => d.TripId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        // Trip → Transportations (1:N)
        builder
            .HasMany(t => t.Transportations)
            .WithOne()
            .HasForeignKey(tp => tp.TripId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}