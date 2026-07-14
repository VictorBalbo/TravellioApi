using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Travellio.Domain;
using Travellio.Domain.Entities;

namespace Travellio.Infrastructure.DbContexts;

public class LegConfiguration : BaseEntityConfiguration<Leg>
{
    public override void Configure(EntityTypeBuilder<Leg> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.DeparturePlaceId)
            .IsRequired()
            .HasMaxLength(Constants.PlaceIdSize);
        builder.Property(p => p.DeparturePlaceShortName)
            .HasMaxLength(200);
        builder.Property(p => p.DeparturePlaceDescription)
            .HasMaxLength(200);
        builder
            .OwnsOne(p => p.DeparturePlaceCoordinates, CoordinatesConfiguration.Configure);
        builder.Property(p => p.ArrivalPlaceId)
            .IsRequired()
            .HasMaxLength(Constants.PlaceIdSize);
        builder.Property(p => p.ArrivalPlaceShortName)
            .HasMaxLength(200);
        builder.Property(p => p.ArrivalPlaceDescription)
            .HasMaxLength(200);
        builder
            .OwnsOne(p => p.ArrivalPlaceCoordinates, CoordinatesConfiguration.Configure);
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
    }
}