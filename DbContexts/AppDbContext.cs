using Microsoft.EntityFrameworkCore;
using TravellioApi.Models;

namespace TravellioApi.DbContexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Trip> Trip { get; set; }
    public DbSet<Destination> Destination { get; set; }
    public DbSet<Transportation> Transportation { get; set; }
    public DbSet<Accommodation> Accommodation { get; set; }
    public DbSet<Activity> Activity { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var accommodationConfiguration = new AccommodationConfiguration();
        modelBuilder.ApplyConfiguration(accommodationConfiguration);
        
        var activityConfiguration = new ActivityConfiguration();
        modelBuilder.ApplyConfiguration(activityConfiguration);
        
        var destinationConfiguration = new DestinationConfiguration();
        modelBuilder.ApplyConfiguration(destinationConfiguration);
        
        var transportationConfiguration = new TransportationConfiguration();
        modelBuilder.ApplyConfiguration(transportationConfiguration);
        
        var transportationSegmentConfigurationConfiguration = new TransportationSegmentConfiguration();
        modelBuilder.ApplyConfiguration(transportationSegmentConfigurationConfiguration);
        
        var tripConfiguration = new TripConfiguration();
        modelBuilder.ApplyConfiguration(tripConfiguration);
    }
}