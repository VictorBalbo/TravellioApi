using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Travellio.Domain.DTOs;
using Travellio.Domain.Entities;
using Travellio.Infrastructure.DbContexts;

namespace Travellio.IntegrationTests;

public class ActivityTests(ApiFactory factory) : IClassFixture<ApiFactory>, IAsyncLifetime
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

    private static Activity NewActivity(Guid destinationId, string name = "Eiffel Tower") => new()
    {
        Id = Guid.CreateVersion7(),
        DestinationId = destinationId,
        Name = name,
        PlaceId = "place-eiffel-tower",
        Coordinates = new Coordinates { Lat = 48.8584m, Lng = 2.2945m },
    };

    private static string BaseUrl(Guid tripId, Guid destinationId)
        => $"/Api/Trips/{tripId}/Destinations/{destinationId}/Activities";

    // GET /Api/Trips/{tripId}/Destinations/{destinationId}/Activities

    [Fact]
    public async Task GetAll_WhenNoActivities_Returns404()
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
    public async Task GetAll_WhenActivitiesExist_Returns200WithList()
    {
        // Arrange
        var trip = NewTrip();
        var destination = NewDestination(trip.Id);
        var activityA = NewActivity(destination.Id, "Pantheon");
        var activityB = NewActivity(destination.Id, "Louvre");
        await SeedAsync(trip, destination, activityA, activityB);

        // Act
        var response = await _client.GetAsync(BaseUrl(trip.Id, destination.Id), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var activities = await response.Content.ReadFromJsonAsync<List<ActivityDto>>(JsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(2, activities!.Count);
        Assert.Contains(activities, a => a.Name == "Pantheon");
        Assert.Contains(activities, a => a.Name == "Louvre");
    }

    // GET /Api/Trips/{tripId}/Destinations/{destinationId}/Activities/{id}

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
        var activity = NewActivity(destination.Id);
        await SeedAsync(trip, destination, activity);

        // Act
        var response = await _client.GetAsync($"{BaseUrl(trip.Id, destination.Id)}/{activity.Id}",
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<ActivityDto>(JsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(activity.Id, dto!.Id);
        Assert.Equal(activity.Name, dto.Name);
        Assert.Equal(activity.PlaceId, dto.PlaceId);
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsEnrichedPlace()
    {
        // Arrange
        var trip = NewTrip();
        var destination = NewDestination(trip.Id);
        var activity = NewActivity(destination.Id);
        await SeedAsync(trip, destination, activity);

        // Act
        var response = await _client.GetAsync($"{BaseUrl(trip.Id, destination.Id)}/{activity.Id}",
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<ActivityDto>(JsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal("Paris, France", dto!.Place?.Address);
    }

    // POST /Api/Trips/{tripId}/Destinations/{destinationId}/Activities

    [Fact]
    public async Task Post_NewActivity_Returns201WithLocationHeader()
    {
        // Arrange
        var trip = NewTrip();
        var destination = NewDestination(trip.Id);
        await SeedAsync(trip, destination);

        var dto = new ActivityDto
        {
            Name = "Eiffel Tower",
            PlaceId = "place-eiffel-tower",
            Coordinates = new Coordinates { Lat = 48.8584m, Lng = 2.2945m },
        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl(trip.Id, destination.Id), dto,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task Post_NewActivity_PersistsToDatabase()
    {
        // Arrange
        var trip = NewTrip();
        var destination = NewDestination(trip.Id);
        await SeedAsync(trip, destination);

        var dto = new ActivityDto
        {
            Name = "Eiffel Tower",
            PlaceId = "place-eiffel-tower",
            Coordinates = new Coordinates { Lat = 48.8584m, Lng = 2.2945m },
        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl(trip.Id, destination.Id), dto,
            cancellationToken: TestContext.Current.CancellationToken);
        var created = await response.Content.ReadFromJsonAsync<ActivityDto>(JsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var saved = await db.Activities.FindAsync(new object?[] { created!.Id }, TestContext.Current.CancellationToken);
        Assert.NotNull(saved);
        Assert.Equal("Eiffel Tower", saved.Name);
    }

    // DELETE /Api/Trips/{tripId}/Destinations/{destinationId}/Activities/{id}

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
        var activity = NewActivity(destination.Id);
        await SeedAsync(trip, destination, activity);

        // Act
        var response = await _client.DeleteAsync($"{BaseUrl(trip.Id, destination.Id)}/{activity.Id}",
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
        var activity = NewActivity(destination.Id);
        await SeedAsync(trip, destination, activity);

        // Act
        await _client.DeleteAsync($"{BaseUrl(trip.Id, destination.Id)}/{activity.Id}",
            TestContext.Current.CancellationToken);

        // Assert
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Null(await db.Activities.FindAsync(new object?[] { activity.Id },
            TestContext.Current.CancellationToken));
    }
}