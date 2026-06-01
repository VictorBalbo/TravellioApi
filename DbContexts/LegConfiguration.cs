using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravellioApi.Models;

namespace TravellioApi.DbContexts;

public class LegConfiguration : IEntityTypeConfiguration<Leg>
{
    public void Configure(EntityTypeBuilder<Leg> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.DeparturePlaceId)
            .IsRequired()
            .HasMaxLength(Constants.PlaceIdSize);
        builder.Property(p => p.ArrivalPlaceId)
            .IsRequired()
            .HasMaxLength(Constants.PlaceIdSize);
        builder.Property(p => p.Type)
            .IsRequired()
            .HasConversion<string>();
        builder.Property(p => p.Company)
            .HasMaxLength(50);
        builder.Property(p => p.ServiceNumber)
            .HasMaxLength(20);
        builder.Property(p => p.Reservation)
            .HasMaxLength(25);
        builder.Property(p => p.Seat)
            .HasMaxLength(10);
        builder.OwnsOne(p => p.Price, PriceConfiguration.Configure);
        builder.Ignore(x => x.DeparturePlace);
        builder.Ignore(x => x.ArrivalPlace);
    }
}