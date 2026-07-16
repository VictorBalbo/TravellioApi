using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Travellio.Domain;
using Travellio.Domain.Entities;

namespace Travellio.Infrastructure.DbContexts;

public class DestinationConfiguration : BaseEntityConfiguration<Destination>
{
    public override void Configure(EntityTypeBuilder<Destination> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.PlaceId)
            .IsRequired()
            .HasMaxLength(Constants.PlaceIdSize);
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(60);
        builder
            .OwnsOne(p => p.Coordinates, CoordinatesConfiguration.Configure);
        builder.Property(p => p.StartDate)
            .IsRequired();
        builder.Property(p => p.EndDate)
            .IsRequired();
        builder.Property(p => p.Notes)
            .HasMaxLength(5000);
        builder.Property(p => p.ImageUrl)
            .HasMaxLength(255);

        // Destination → Activities (1:N)
        builder
            .HasMany(d => d.Activities)
            .WithOne()
            .HasForeignKey(a => a.DestinationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Destination → Accommodations (1:N)
        builder
            .HasMany(d => d.Accommodations)
            .WithOne()
            .HasForeignKey(a => a.DestinationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}