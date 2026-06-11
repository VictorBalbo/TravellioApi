using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravellioApi.Models.Entities;

namespace TravellioApi.DbContexts;

public class ActivityConfiguration : BaseEntityConfiguration<Activity>
{
    public override void Configure(EntityTypeBuilder<Activity> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(p => p.PlaceId)
            .IsRequired()
            .HasMaxLength(Constants.PlaceIdSize);
        builder
            .OwnsOne(p => p.Coordinates, CoordinatesConfiguration.Configure);
        builder.Property(p => p.Address)
            .HasMaxLength(200);
        builder.Property(p => p.Website)
            .HasMaxLength(255);
        builder.Property(p => p.Notes)
            .HasMaxLength(5000);
        builder.OwnsOne(p => p.Price, PriceConfiguration.Configure);
    }
}