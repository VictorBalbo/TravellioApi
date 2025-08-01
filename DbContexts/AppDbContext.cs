using Microsoft.EntityFrameworkCore;
using Travellio.Models;

namespace Travellio.DbContexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Trip> Trip { get; set; }
    public DbSet<Destination> Destination { get; set; }
    public DbSet<Transportation> Transportation { get; set; }
    public DbSet<Accommodation> Accommodation { get; set; }
    public DbSet<Activity> Activity { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Trip → Destinations (1:N)
        modelBuilder.Entity<Trip>()
            .HasMany(t => t.Destinations)
            .WithOne(d => d.Trip)
            .HasForeignKey(d => d.TripId)
            .OnDelete(DeleteBehavior.Cascade);

        // Destination → Activities (1:N)
        modelBuilder.Entity<Destination>()
            .HasMany(d => d.Activities)
            .WithOne(a => a.Destination)
            .HasForeignKey(a => a.DestinationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Destination → Accommodation (1:N)
        modelBuilder.Entity<Destination>()
            .HasMany(d => d.Accommodations)
            .WithOne(a => a.Destination)
            .HasForeignKey(a => a.DestinationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Trip → Transportations (1:N)
        modelBuilder.Entity<Trip>()
            .HasMany(t => t.Transportations)
            .WithOne(tp => tp.Trip)
            .HasForeignKey(tp => tp.TripId)
            .OnDelete(DeleteBehavior.Cascade);

        // Transportation → Origin (N:1)
        modelBuilder.Entity<Transportation>()
            .HasOne(tp => tp.Origin)
            .WithMany()
            .HasForeignKey(tp => tp.OriginId)
            .OnDelete(DeleteBehavior.Restrict);

        // Transportation → Destination (N:1)
        modelBuilder.Entity<Transportation>()
            .HasOne(tp => tp.Destination)
            .WithMany()
            .HasForeignKey(tp => tp.DestinationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Transportation → TransportationSegments (1:N)
        modelBuilder.Entity<Transportation>()
            .HasMany(t => t.Segments)
            .WithOne(ts => ts.Transportation)
            .HasForeignKey(ts => ts.TransportationId)
            .OnDelete(DeleteBehavior.Cascade);

        // TransportationSegment save Type as string
        modelBuilder.Entity<TransportationSegment>()
            .Property(t => t.Type)
            .HasConversion<string>();
    }
}
