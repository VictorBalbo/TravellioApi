using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Travellio.Domain.DTOs;
using Travellio.Domain.Entities;
using Travellio.Infrastructure.DbContexts;

namespace Travellio.IntegrationTests;

public class AccommodationTests(ApiFactory factory) : IClassFixture<ApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public async ValueTask InitializeAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Trips.RemoveRange(db.Trips);
        await db.SaveChangesAsync();
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

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

    private static Accommodation NewAccommodation(Guid destinationId, string name = "Hotel de Ville") => new()
    {
        Id = Guid.CreateVersion7(),
        DestinationId = destinationId,
        Name = name,
        PlaceId = "place-hotel-paris",
        Coordinates = new Coordinates { Lat = 48.8566m, Lng = 2.3522m },
    };

    private static string BaseUrl(Guid tripId, Guid destinationId)
        => $"/Api/Trips/{tripId}/Destinations/{destinationId}/Accommodations";

    // GET /Api/Trips/{tripId}/Destinations/{destinationId}/Accommodations

    [Fact]
    public async Task GetAll_WhenNoAccommodations_Returns404()
    {
        // Arrange
        var trip = NewTrip();
        var destination = NewDestination(trip.Id);
        await SeedAsync(trip, destination);

        // Act
        var response = await _client.GetAsync(BaseUrl(trip.Id, destination.Id), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WhenAccommodationsExist_Returns200WithList()
    {
        // Arrange
        var trip = NewTrip();
        var destination = NewDestination(trip.Id);
        var accommodationA = NewAccommodation(destination.Id, "Le Bristol");
        var accommodationB = NewAccommodation(destination.Id, "Le Marais Suites");
        await SeedAsync(trip, destination, accommodationA, accommodationB);

        // Act
        var response = await _client.GetAsync(BaseUrl(trip.Id, destination.Id), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var accommodations = await response.Content.ReadFromJsonAsync<List<AccommodationDto>>(JsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(2, accommodations!.Count);
        Assert.Contains(accommodations, a => a.Name == "Le Bristol");
        Assert.Contains(accommodations, a => a.Name == "Le Marais Suites");
    }

    // GET /Api/Trips/{tripId}/Destinations/{destinationId}/Accommodations/{id}

    [Fact]
    public async Task GetById_WhenNotFound_Returns404()
    {
        // Arrange
        var trip = NewTrip();
        var destination = NewDestination(trip.Id);
        await SeedAsync(trip, destination);

        // Act
        var response = await _client.GetAsync($"{BaseUrl(trip.Id, destination.Id)}/{Guid.CreateVersion7()}",
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenExists_Returns200WithData()
    {
        // Arrange
        var trip = NewTrip();
        var destination = NewDestination(trip.Id);
        var accommodation = NewAccommodation(destination.Id);
        await SeedAsync(trip, destination, accommodation);

        // Act
        var response = await _client.GetAsync($"{BaseUrl(trip.Id, destination.Id)}/{accommodation.Id}",
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<AccommodationDto>(JsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(accommodation.Id, dto!.Id);
        Assert.Equal(accommodation.Name, dto.Name);
        Assert.Equal(accommodation.PlaceId, dto.PlaceId);
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsEnrichedPlace()
    {
        // Arrange
        var trip = NewTrip();
        var destination = NewDestination(trip.Id);
        var accommodation = NewAccommodation(destination.Id);
        await SeedAsync(trip, destination, accommodation);

        // Act
        var response = await _client.GetAsync($"{BaseUrl(trip.Id, destination.Id)}/{accommodation.Id}",
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<AccommodationDto>(JsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal("Paris, France", dto!.Place?.Address);
    }

    // POST /Api/Trips/{tripId}/Destinations/{destinationId}/Accommodations

    [Fact]
    public async Task Post_NewAccommodation_Returns201WithLocationHeader()
    {
        // Arrange
        var trip = NewTrip();
        var destination = NewDestination(trip.Id);
        await SeedAsync(trip, destination);

        var dto = new AccommodationDto
        {
            Name = "Hotel de Ville",
            PlaceId = "place-hotel-paris",
            Coordinates = new Coordinates { Lat = 48.8566m, Lng = 2.3522m },
        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl(trip.Id, destination.Id), dto,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task Post_NewAccommodation_PersistsToDatabase()
    {
        // Arrange
        var trip = NewTrip();
        var destination = NewDestination(trip.Id);
        await SeedAsync(trip, destination);

        var dto = new AccommodationDto
        {
            Name = "Hotel de Ville",
            PlaceId = "place-hotel-paris",
            Coordinates = new Coordinates { Lat = 48.8566m, Lng = 2.3522m },
        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl(trip.Id, destination.Id), dto,
            cancellationToken: TestContext.Current.CancellationToken);
        var created = await response.Content.ReadFromJsonAsync<AccommodationDto>(JsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var saved = await db.Accommodations.FindAsync(new object?[] { created!.Id },
            TestContext.Current.CancellationToken);
        Assert.NotNull(saved);
        Assert.Equal("Hotel de Ville", saved.Name);
    }

    // DELETE /Api/Trips/{tripId}/Destinations/{destinationId}/Accommodations/{id}

    [Fact]
    public async Task Delete_WhenNotFound_Returns404()
    {
        // Arrange
        var trip = NewTrip();
        var destination = NewDestination(trip.Id);
        await SeedAsync(trip, destination);

        // Act
        var response = await _client.DeleteAsync($"{BaseUrl(trip.Id, destination.Id)}/{Guid.CreateVersion7()}",
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenExists_Returns204()
    {
        // Arrange
        var trip = NewTrip();
        var destination = NewDestination(trip.Id);
        var accommodation = NewAccommodation(destination.Id);
        await SeedAsync(trip, destination, accommodation);

        // Act
        var response = await _client.DeleteAsync($"{BaseUrl(trip.Id, destination.Id)}/{accommodation.Id}",
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenExists_RemovesFromDatabase()
    {
        // Arrange
        var trip = NewTrip();
        var destination = NewDestination(trip.Id);
        var accommodation = NewAccommodation(destination.Id);
        await SeedAsync(trip, destination, accommodation);

        // Act
        await _client.DeleteAsync($"{BaseUrl(trip.Id, destination.Id)}/{accommodation.Id}",
            TestContext.Current.CancellationToken);

        // Assert
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Null(await db.Accommodations.FindAsync(new object?[] { accommodation.Id },
            TestContext.Current.CancellationToken));
    }
}