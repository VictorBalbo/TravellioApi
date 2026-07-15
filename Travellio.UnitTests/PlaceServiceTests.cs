using NSubstitute;
using Serilog;
using Travellio.Api.Services;
using Travellio.Api.Services.PlaceProviders;
using Travellio.Domain.DTOs;
using Travellio.Domain.Entities;

namespace Travellio.Tests;

public class PlaceServiceTests
{
    private readonly IPlaceProvider _external = Substitute.For<IPlaceProvider>();
    private readonly ICachedPlaceProvider _cache = Substitute.For<ICachedPlaceProvider>();
    private readonly IDiagnosticContext _diagnosticContext = Substitute.For<IDiagnosticContext>();
    private readonly PlaceService _placeService;

    private static readonly Place ValidPlace = new()
    {
        Id = "place-1",
        Name = "Eiffel Tower",
        Coordinates = new Coordinates { Lat = 48.8584m, Lng = 2.2945m },
    };

    private static readonly AutoComplete[] AutoCompleteResults =
    [
        new() { PlaceId = "place-1", MainText = "Eiffel Tower" }
    ];

    public PlaceServiceTests()
    {
        _placeService = new PlaceService(_external, _cache, _diagnosticContext);
    }

    // GetPlaceDetails

    [Fact]
    public async Task GetPlaceDetails_CacheHit_ReturnsCachedPlace()
    {
        // Arrange
        _cache.GetPlaceDetailsAsync("place-1", CancellationToken.None).Returns(ValidPlace);

        // Act
        var result = await _placeService.GetPlaceDetails("place-1", CancellationToken.None);

        // Assert
        Assert.Equal(ValidPlace, result);
        _diagnosticContext.Received(1).Set("CacheResult", "Hit");
        await _external.DidNotReceive().GetPlaceDetailsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetPlaceDetails_CacheHit_DoesNotWriteToCache()
    {
        // Arrange
        _cache.GetPlaceDetailsAsync("place-1", CancellationToken.None).Returns(ValidPlace);

        // Act
        await _placeService.GetPlaceDetails("place-1", CancellationToken.None);

        // Assert
        await _cache.DidNotReceive().SetPlaceDetailsAsync(Arg.Any<Place>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetPlaceDetails_CacheMiss_ExternalReturnsValidPlace_StoresAndReturnsPlace()
    {
        // Arrange
        _cache.GetPlaceDetailsAsync("place-1", CancellationToken.None).Returns((Place?)null);
        _external.GetPlaceDetailsAsync("place-1", CancellationToken.None).Returns(ValidPlace);

        // Act
        var result = await _placeService.GetPlaceDetails("place-1", CancellationToken.None);

        // Assert
        Assert.Equal(ValidPlace, result);
        _diagnosticContext.Received(1).Set("CacheResult", "Miss");
        await _cache.Received(1).SetPlaceDetailsAsync(ValidPlace, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetPlaceDetails_CacheMiss_ExternalReturnsPlaceWithEmptyName_ReturnsNull()
    {
        // Arrange
        var namelessPlace = new Place { Id = "place-1", Name = "", Coordinates = new Coordinates() };
        _cache.GetPlaceDetailsAsync("place-1", CancellationToken.None).Returns((Place?)null);
        _external.GetPlaceDetailsAsync("place-1", CancellationToken.None).Returns(namelessPlace);

        // Act
        var result = await _placeService.GetPlaceDetails("place-1", CancellationToken.None);

        // Assert
        Assert.Null(result);
        await _cache.DidNotReceive().SetPlaceDetailsAsync(Arg.Any<Place>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetPlaceDetails_CacheMiss_ExternalReturnsNull_ReturnsNull()
    {
        // Arrange
        _cache.GetPlaceDetailsAsync("place-1", CancellationToken.None).Returns((Place?)null);
        _external.GetPlaceDetailsAsync("place-1", CancellationToken.None).Returns((Place?)null);

        // Act
        var result = await _placeService.GetPlaceDetails("place-1", CancellationToken.None);

        // Assert
        Assert.Null(result);
        await _cache.DidNotReceive().SetPlaceDetailsAsync(Arg.Any<Place>(), Arg.Any<CancellationToken>());
    }

    // GetAutoComplete

    [Fact]
    public async Task GetAutoComplete_CacheHit_ReturnsCachedResults()
    {
        // Arrange
        _cache.GetAutoCompleteAsync("paris", "token", 0, 0, 0, "en", "", CancellationToken.None)
            .Returns(AutoCompleteResults);

        // Act
        var result = await _placeService.GetAutoComplete("paris", "token", 0, 0, 0, "en", "", CancellationToken.None);

        // Assert
        Assert.Equal(AutoCompleteResults, result);
        _diagnosticContext.Received(1).Set("CacheResult", "Hit");
        await _external.DidNotReceive()
            .GetAutoCompleteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<double>(), Arg.Any<double>(),
                Arg.Any<double>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAutoComplete_CacheHit_DoesNotWriteToCache()
    {
        // Arrange
        _cache.GetAutoCompleteAsync("paris", "token", 0, 0, 0, "en", "", CancellationToken.None)
            .Returns(AutoCompleteResults);

        // Act
        await _placeService.GetAutoComplete("paris", "token", 0, 0, 0, "en", "", CancellationToken.None);

        // Assert
        await _cache.DidNotReceive()
            .SetAutoCompleteAsync(Arg.Any<IEnumerable<AutoComplete>>(), Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAutoComplete_CacheMiss_ExternalReturnsResults_StoresAndReturnsResults()
    {
        // Arrange
        _cache.GetAutoCompleteAsync("paris", "token", 0, 0, 0, "en", "", CancellationToken.None)
            .Returns((IEnumerable<AutoComplete>?)null);
        _external.GetAutoCompleteAsync("paris", "token", 0, 0, 0, "en", "", CancellationToken.None)
            .Returns(AutoCompleteResults);

        // Act
        var result = await _placeService.GetAutoComplete("paris", "token", 0, 0, 0, "en", "", CancellationToken.None);

        // Assert
        Assert.Equal(AutoCompleteResults, result);
        _diagnosticContext.Received(1).Set("CacheResult", "Miss");
        await _cache.Received(1)
            .SetAutoCompleteAsync(Arg.Any<IEnumerable<AutoComplete>>(), "paris", "en", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAutoComplete_CacheMiss_ExternalReturnsEmptyList_ReturnsNull()
    {
        // Arrange
        _cache.GetAutoCompleteAsync("paris", "token", 0, 0, 0, "en", "", CancellationToken.None)
            .Returns((IEnumerable<AutoComplete>?)null);
        _external.GetAutoCompleteAsync("paris", "token", 0, 0, 0, "en", "", CancellationToken.None)
            .Returns(Array.Empty<AutoComplete>());

        // Act
        var result = await _placeService.GetAutoComplete("paris", "token", 0, 0, 0, "en", "", CancellationToken.None);

        // Assert
        Assert.Null(result);
        await _cache.DidNotReceive()
            .SetAutoCompleteAsync(Arg.Any<IEnumerable<AutoComplete>>(), Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAutoComplete_CacheMiss_ExternalReturnsNull_ReturnsNull()
    {
        // Arrange
        _cache.GetAutoCompleteAsync("paris", "token", 0, 0, 0, "en", "", CancellationToken.None)
            .Returns((IEnumerable<AutoComplete>?)null);
        _external.GetAutoCompleteAsync("paris", "token", 0, 0, 0, "en", "", CancellationToken.None)
            .Returns((IEnumerable<AutoComplete>?)null);

        // Act
        var result = await _placeService.GetAutoComplete("paris", "token", 0, 0, 0, "en", "", CancellationToken.None);

        // Assert
        Assert.Null(result);
        await _cache.DidNotReceive()
            .SetAutoCompleteAsync(Arg.Any<IEnumerable<AutoComplete>>(), Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<CancellationToken>());
    }
}