using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravellioApi.Models.Entities;

namespace TravellioApi.DbContexts;

public class AccommodationConfiguration : BaseEntityConfiguration<Accommodation>
{
    public override void Configure(EntityTypeBuilder<Accommodation> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(p => p.PlaceId)
            .IsRequired()
            .HasMaxLength(Constants.PlaceIdSize);
        builder.Property(p => p.ImageUrl)
            .HasMaxLength(255);
        builder.Property(p => p.Website)
            .HasMaxLength(255);
        builder.Property(p => p.Notes)
            .HasMaxLength(5000);
        builder.OwnsOne(p => p.Price, PriceConfiguration.Configure);
    }
}