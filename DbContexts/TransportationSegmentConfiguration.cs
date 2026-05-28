using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravellioApi.Models;

namespace TravellioApi.DbContexts;

public class TransportationSegmentConfiguration : IEntityTypeConfiguration<TransportationSegment>
{
    public void Configure(EntityTypeBuilder<TransportationSegment> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.OriginTerminalPlaceId)
            .IsRequired()
            .HasMaxLength(Constants.PlaceIdSize);
        builder.Property(p => p.DestinationTerminalPlaceId)
            .IsRequired()
            .HasMaxLength(Constants.PlaceIdSize);
        builder.Property(p => p.Type)
            .IsRequired();
        builder.Property(p => p.Company)
            .HasMaxLength(50);
        builder.Property(p => p.TransportIdentification)
            .HasMaxLength(10);
        builder.Property(p => p.Reservation)
            .HasMaxLength(10);
        builder.Property(p => p.Seat)
            .HasMaxLength(10);
        builder.OwnsOne(p => p.Price, PriceConfiguration.Configure);
        builder.Ignore(x => x.OriginTerminal);
        builder.Ignore(x => x.DestinationTerminal);
        builder
            .Property(t => t.Type)
            .HasConversion<string>();
    }
}