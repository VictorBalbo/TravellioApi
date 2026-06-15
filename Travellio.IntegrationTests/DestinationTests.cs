using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Travellio.Domain.DTOs;
using Travellio.Domain.Entities;
using Travellio.Infrastructure.DbContexts;

namespace Travellio.IntegrationTests;

public class DestinationTests(ApiFactory factory) : IClassFixture<ApiFactory>, IAsyncLifetime
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

    private static Trip NewTrip() => new()
    {
        Id = Guid.CreateVersion7(),
        Name = "Test Trip",
        StartDate = new DateOnly(2026, 6, 1),
        EndDate = new DateOnly(2026, 6, 15),
    };

    private static Destination NewDestination(Guid tripId, string name = "Paris") => new()
    {
        Id = Guid.CreateVersion7(),
        TripId = tripId,
        PlaceId = "place-paris",
        Name = name,
        Coordinates = new Coordinates { Lat = 48.8566m, Lng = 2.3522m },
        StartDate = new DateOnly(2026, 6, 1),
        EndDate = new DateOnly(2026, 6, 7),
    };

    // GET /Api/Trips/{tripId}/Destinations

    [Fact]
    public async Task GetAll_WhenNoDestinations_Returns404()
    {
        // Arrange
        var trip = NewTrip();
        await SeedAsync(trip);

        // Act
        var response = await _client.GetAsync($"/Api/Trips/{trip.Id}/Destinations");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WhenDestinationsExist_Returns200WithList()
    {
        // Arrange
        var trip = NewTrip();
        var destA = NewDestination(trip.Id, "Barcelona");
        var destB = NewDestination(trip.Id, "Rome");
        await SeedAsync(trip, destA, destB);

        // Act
        var response = await _client.GetAsync($"/Api/Trips/{trip.Id}/Destinations");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var destinations = await response.Content.ReadFromJsonAsync<List<DestinationDto>>(JsonOptions);
        Assert.Equal(2, destinations!.Count);
        Assert.Contains(destinations, d => d.Name == "Barcelona");
        Assert.Contains(destinations, d => d.Name == "Rome");
    }

    // GET /Api/Trips/{tripId}/Destinations/{id}

    [Fact]
    public async Task GetById_WhenNotFound_Returns404()
    {
        // Arrange
        var trip = NewTrip();
        await SeedAsync(trip);

        // Act
        var response = await _client.GetAsync($"/Api/Trips/{trip.Id}/Destinations/{Guid.CreateVersion7()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenExists_Returns200WithData()
    {
        // Arrange
        var trip = NewTrip();
        var destination = NewDestination(trip.Id);
        await SeedAsync(trip, destination);

        // Act
        var response = await _client.GetAsync($"/Api/Trips/{trip.Id}/Destinations/{destination.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<DestinationDto>(JsonOptions);
        Assert.Equal(destination.Id, dto!.Id);
        Assert.Equal(destination.Name, dto.Name);
        Assert.Equal(destination.PlaceId, dto.PlaceId);
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsEnrichedPlace()
    {
        // Arrange
        var trip = NewTrip();
        var destination = NewDestination(trip.Id);
        await SeedAsync(trip, destination);

        // Act
        var response = await _client.GetAsync($"/Api/Trips/{trip.Id}/Destinations/{destination.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<DestinationDto>(JsonOptions);
        Assert.Equal("Paris, France", dto!.Place?.Address);
    }

    // POST /Api/Trips/{tripId}/Destinations

    [Fact]
    public async Task Post_NewDestination_Returns201WithLocationHeader()
    {
        // Arrange
        var trip = NewTrip();
        await SeedAsync(trip);

        var dto = new DestinationDto
        {
            PlaceId = "place-paris",
            Name = "Paris",
            Coordinates = new Coordinates { Lat = 48.8566m, Lng = 2.3522m },
            StartDate = new DateOnly(2026, 6, 1),
            EndDate = new DateOnly(2026, 6, 7),
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/Api/Trips/{trip.Id}/Destinations", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task Post_NewDestination_PersistsToDatabase()
    {
        // Arrange
        var trip = NewTrip();
        await SeedAsync(trip);

        var dto = new DestinationDto
        {
            PlaceId = "place-paris",
            Name = "Paris",
            Coordinates = new Coordinates { Lat = 48.8566m, Lng = 2.3522m },
            StartDate = new DateOnly(2026, 6, 1),
            EndDate = new DateOnly(2026, 6, 7),
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/Api/Trips/{trip.Id}/Destinations", dto);
        var created = await response.Content.ReadFromJsonAsync<DestinationDto>(JsonOptions);

        // Assert
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var saved = await db.Destinations.FindAsync(created!.Id);
        Assert.NotNull(saved);
        Assert.Equal("Paris", saved.Name);
    }

    [Fact]
    public async Task Post_ExistingDestination_UpdatesName()
    {
        // Arrange
        var trip = NewTrip();
        var destination = NewDestination(trip.Id, "Original Name");
        await SeedAsync(trip, destination);

        var updated = new DestinationDto
        {
            Id = destination.Id,
            PlaceId = destination.PlaceId,
            Name = "Updated Name",
            Coordinates = destination.Coordinates,
            StartDate = destination.StartDate,
            EndDate = destination.EndDate,
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/Api/Trips/{trip.Id}/Destinations", updated);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var saved = await db.Destinations.FindAsync(destination.Id);
        Assert.Equal("Updated Name", saved!.Name);
    }

    // DELETE /Api/Trips/{tripId}/Destinations/{id}

    [Fact]
    public async Task Delete_WhenNotFound_Returns404()
    {
        // Arrange
        var trip = NewTrip();
        await SeedAsync(trip);

        // Act
        var response = await _client.DeleteAsync($"/Api/Trips/{trip.Id}/Destinations/{Guid.CreateVersion7()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenExists_Returns204()
    {
        // Arrange
        var trip = NewTrip();
        var destination = NewDestination(trip.Id);
        await SeedAsync(trip, destination);

        // Act
        var response = await _client.DeleteAsync($"/Api/Trips/{trip.Id}/Destinations/{destination.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenExists_RemovesFromDatabase()
    {
        // Arrange
        var trip = NewTrip();
        var destination = NewDestination(trip.Id);
        await SeedAsync(trip, destination);

        // Act
        await _client.DeleteAsync($"/Api/Trips/{trip.Id}/Destinations/{destination.Id}");

        // Assert
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Null(await db.Destinations.FindAsync(destination.Id));
    }
}