using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravellioApi.Models;

namespace TravellioApi.DbContexts;

public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(p => p.PlaceId)
            .IsRequired()
            .HasMaxLength(Constants.PlaceIdSize);
        builder.Property(p => p.Website)
            .HasMaxLength(255);
        builder.Property(p => p.Notes)
            .HasMaxLength(5000);
        builder.OwnsOne(p => p.Price, PriceConfiguration.Configure);
        builder.Ignore(x => x.Place);
    }
}