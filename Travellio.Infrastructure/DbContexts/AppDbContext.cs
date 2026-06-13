using Microsoft.EntityFrameworkCore;
using Travellio.Domain.Entities;

namespace Travellio.Infrastructure.DbContexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Trip> Trips { get; set; }
    public DbSet<Destination> Destinations { get; set; }
    public DbSet<Transportation> Transportations { get; set; }
    public DbSet<Accommodation> Accommodations { get; set; }
    public DbSet<Activity> Activities { get; set; }
    public DbSet<Leg> Legs { get; set; }

    public DbSet<Airport> Airports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}