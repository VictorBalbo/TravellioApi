using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Travellio.Domain.DTOs;
using Travellio.Domain.Entities;
using Travellio.Infrastructure.DbContexts;

namespace Travellio.IntegrationTests;

public class TripTests(ApiFactory factory) : IClassFixture<ApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task InitializeAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Trips.RemoveRange(db.Trips);
        await db.SaveChangesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task SeedAsync(params object[] entities)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.AddRange(entities);
        await db.SaveChangesAsync();
    }

    private static Trip NewTrip(string name = "Test Trip") => new()
    {
        Id = Guid.CreateVersion7(),
        Name = name,
        StartDate = new DateOnly(2026, 6, 1),
        EndDate = new DateOnly(2026, 6, 15),
    };

    private static Destination NewDestination(Guid tripId) => new()
    {
        Id = Guid.CreateVersion7(),
        TripId = tripId,
        PlaceId = "place-paris",
        Name = "Paris",
        Coordinates = new Coordinates { Lat = 48.8566m, Lng = 2.3522m },
        StartDate = new DateOnly(2026, 6, 1),
        EndDate = new DateOnly(2026, 6, 7),
    };

    // GET /Api/Trips

    [Fact]
    public async Task GetAll_WhenNoTrips_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/Api/Trips");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WhenTripsExist_Returns200WithList()
    {
        // Arrange
        await SeedAsync(NewTrip("Trip A"), NewTrip("Trip B"));

        // Act
        var response = await _client.GetAsync("/Api/Trips");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var trips = await response.Content.ReadFromJsonAsync<List<TripDto>>(JsonOptions);
        Assert.Equal(2, trips!.Count);
        Assert.Contains(trips, t => t.Name == "Trip A");
        Assert.Contains(trips, t => t.Name == "Trip B");
    }

    // GET /Api/Trips/{id}

    [Fact]
    public async Task GetById_WhenNotFound_Returns404()
    {
        // Act
        var response = await _client.GetAsync($"/Api/Trips/{Guid.CreateVersion7()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenExists_Returns200WithTripData()
    {
        // Arrange
        var trip = NewTrip("Paris 2026");
        await SeedAsync(trip);

        // Act
        var response = await _client.GetAsync($"/Api/Trips/{trip.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<TripDto>(JsonOptions);
        Assert.Equal(trip.Id, dto!.Id);
        Assert.Equal("Paris 2026", dto.Name);
        Assert.Equal(trip.StartDate, dto.StartDate);
        Assert.Equal(trip.EndDate, dto.EndDate);
    }

    [Fact]
    public async Task GetById_WhenTripHasDestination_IncludesDestinationEnriched()
    {
        // Arrange
        var trip = NewTrip();
        var destination = NewDestination(trip.Id);
        await SeedAsync(trip, destination);

        // Act
        var response = await _client.GetAsync($"/Api/Trips/{trip.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<TripDto>(JsonOptions);
        var dest = Assert.Single(dto!.Destinations!);
        Assert.Equal(destination.Name, dest.Name);
        Assert.Equal(destination.PlaceId, dest.PlaceId);
        Assert.Equal("Paris, France", dest.Place?.Address);
    }

    // POST /Api/Trips

    [Fact]
    public async Task Post_NewTrip_Returns201WithLocationHeader()
    {
        // Arrange
        var dto = new TripDto
        {
            Name = "New Trip",
            StartDate = new DateOnly(2026, 7, 1),
            EndDate = new DateOnly(2026, 7, 14),
        };

        // Act
        var response = await _client.PostAsJsonAsync("/Api/Trips", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task Post_NewTrip_PersistsToDatabase()
    {
        // Arrange
        var dto = new TripDto
        {
            Name = "Persisted Trip",
            StartDate = new DateOnly(2026, 8, 1),
            EndDate = new DateOnly(2026, 8, 10),
        };

        // Act
        var response = await _client.PostAsJsonAsync("/Api/Trips", dto);
        var created = await response.Content.ReadFromJsonAsync<TripDto>(JsonOptions);

        // Assert
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var saved = await db.Trips.FindAsync(created!.Id);
        Assert.NotNull(saved);
        Assert.Equal("Persisted Trip", saved.Name);
    }

    [Fact]
    public async Task Post_ExistingTrip_UpdatesName()
    {
        // Arrange
        var trip = NewTrip("Original Name");
        await SeedAsync(trip);

        var updated = new TripDto
        {
            Id = trip.Id,
            Name = "Updated Name",
            StartDate = trip.StartDate,
            EndDate = trip.EndDate,
        };

        // Act
        var response = await _client.PostAsJsonAsync("/Api/Trips", updated);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var saved = await db.Trips.FindAsync(trip.Id);
        Assert.Equal("Updated Name", saved!.Name);
    }

    // DELETE /Api/Trips/{id}

    [Fact]
    public async Task Delete_WhenNotFound_Returns404()
    {
        // Act
        var response = await _client.DeleteAsync($"/Api/Trips/{Guid.CreateVersion7()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenExists_Returns204()
    {
        // Arrange
        var trip = NewTrip();
        await SeedAsync(trip);

        // Act
        var response = await _client.DeleteAsync($"/Api/Trips/{trip.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenExists_RemovesFromDatabase()
    {
        // Arrange
        var trip = NewTrip();
        await SeedAsync(trip);

        // Act
        await _client.DeleteAsync($"/Api/Trips/{trip.Id}");

        // Assert
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Null(await db.Trips.FindAsync(trip.Id));
    }

    [Fact]
    public async Task Delete_WhenExists_CascadeDeletesDestinations()
    {
        // Arrange
        var trip = NewTrip();
        var destination = NewDestination(trip.Id);
        await SeedAsync(trip, destination);

        // Act
        await _client.DeleteAsync($"/Api/Trips/{trip.Id}");

        // Assert
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Empty(db.Destinations.Where(d => d.TripId == trip.Id));
    }
}