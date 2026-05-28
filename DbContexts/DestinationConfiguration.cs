using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravellioApi.Models;

namespace TravellioApi.DbContexts;

public class DestinationConfiguration : IEntityTypeConfiguration<Destination>
{
    public void Configure(EntityTypeBuilder<Destination> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.PlaceId)
            .IsRequired()
            .HasMaxLength(Constants.PlaceIdSize);
        builder.Property(p => p.StartDate)
            .IsRequired();
        builder.Property(p => p.EndDate)
            .IsRequired();
        builder.Property(p => p.Notes)
            .HasMaxLength(5000);
        builder.Ignore(x => x.Place);
        
        // Destination → Activities (1:N)
        builder
            .HasMany(d => d.Activities)
                .WithOne()
            .HasForeignKey(a => a.DestinationId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Destination → Accommodation (1:N)
        builder
            .HasMany(d => d.Accommodations)
            .WithOne()
            .HasForeignKey(a => a.DestinationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}