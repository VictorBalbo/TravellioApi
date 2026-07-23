using Microsoft.Extensions.Logging;
using NSubstitute;
using Serilog;
using Travellio.Api.Services;
using Travellio.Api.Services.PlaceProviders;
using Travellio.Domain.DTOs;
using Travellio.Domain.Entities;

namespace Travellio.Tests;

public class PlaceServiceTests
{
    private readonly IPlaceProvider _primary = Substitute.For<IPlaceProvider>();
    private readonly IPlaceProvider _secondary = Substitute.For<IPlaceProvider>();
    private readonly ICachedPlaceProvider _cache = Substitute.For<ICachedPlaceProvider>();
    private readonly IDiagnosticContext _diagnosticContext = Substitute.For<IDiagnosticContext>();
    private readonly ILogger<PlaceService> _logger = Substitute.For<ILogger<PlaceService>>();
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
        _cache.ProviderName.Returns(nameof(CachedPlaceProvider));
        _cache.Priority.Returns(0);
        _primary.Priority.Returns(1);
        _secondary.Priority.Returns(2);

        _placeService = new PlaceService(_cache, [_primary, _secondary], _diagnosticContext, _logger);
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
        await _primary.DidNotReceive().GetPlaceDetailsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _secondary.DidNotReceive().GetPlaceDetailsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
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
    public async Task GetPlaceDetails_CacheMiss_PrimaryReturnsValidPlace_StoresAndReturnsPlace()
    {
        // Arrange
        _cache.GetPlaceDetailsAsync("place-1", CancellationToken.None).Returns((Place?)null);
        _primary.GetPlaceDetailsAsync("place-1", CancellationToken.None).Returns(ValidPlace);

        // Act
        var result = await _placeService.GetPlaceDetails("place-1", CancellationToken.None);

        // Assert
        Assert.Equal(ValidPlace, result);
        await _cache.Received(1).SetPlaceDetailsAsync(ValidPlace, Arg.Any<CancellationToken>());
        await _secondary.DidNotReceive().GetPlaceDetailsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetPlaceDetails_CacheAndPrimaryMiss_SecondaryReturnsValidPlace_StoresAndReturnsPlace()
    {
        // Arrange
        _cache.GetPlaceDetailsAsync("place-1", CancellationToken.None).Returns((Place?)null);
        _primary.GetPlaceDetailsAsync("place-1", CancellationToken.None).Returns((Place?)null);
        _secondary.GetPlaceDetailsAsync("place-1", CancellationToken.None).Returns(ValidPlace);

        // Act
        var result = await _placeService.GetPlaceDetails("place-1", CancellationToken.None);

        // Assert
        Assert.Equal(ValidPlace, result);
        await _cache.Received(1).SetPlaceDetailsAsync(ValidPlace, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetPlaceDetails_PrimaryThrows_FallsBackToSecondary()
    {
        // Arrange
        _cache.GetPlaceDetailsAsync("place-1", CancellationToken.None).Returns((Place?)null);
        _primary.GetPlaceDetailsAsync("place-1", CancellationToken.None)
            .Returns(Task.FromException<Place?>(new HttpRequestException()));
        _secondary.GetPlaceDetailsAsync("place-1", CancellationToken.None).Returns(ValidPlace);

        // Act
        var result = await _placeService.GetPlaceDetails("place-1", CancellationToken.None);

        // Assert
        Assert.Equal(ValidPlace, result);
    }

    [Fact]
    public async Task GetPlaceDetails_ProvidersRegisteredOutOfOrder_StillTriesLowerPriorityFirst()
    {
        // Arrange: constructed with secondary (priority 2) before primary (priority 1) -
        // the service must sort by Priority rather than trust constructor argument order.
        var placeService = new PlaceService(_cache, [_secondary, _primary], _diagnosticContext, _logger);
        _cache.GetPlaceDetailsAsync("place-1", CancellationToken.None).Returns((Place?)null);
        _primary.GetPlaceDetailsAsync("place-1", CancellationToken.None).Returns(ValidPlace);
        _secondary.GetPlaceDetailsAsync("place-1", CancellationToken.None).Returns(ValidPlace);

        // Act
        await placeService.GetPlaceDetails("place-1", CancellationToken.None);

        // Assert
        await _secondary.DidNotReceive().GetPlaceDetailsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _primary.Received(1).GetPlaceDetailsAsync("place-1", CancellationToken.None);
    }

    [Fact]
    public async Task GetPlaceDetails_AllProvidersMiss_ReturnsNull()
    {
        // Arrange
        _cache.GetPlaceDetailsAsync("place-1", CancellationToken.None).Returns((Place?)null);
        _primary.GetPlaceDetailsAsync("place-1", CancellationToken.None).Returns((Place?)null);
        _secondary.GetPlaceDetailsAsync("place-1", CancellationToken.None).Returns((Place?)null);

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
        _cache.GetAutoCompleteAsync("paris", "token", 0, 0, 0, "en", CancellationToken.None)
            .Returns(AutoCompleteResults);

        // Act
        var result = await _placeService.GetAutoComplete("paris", "token", 0, 0, 0, "en", "",
            CancellationToken.None);

        // Assert
        Assert.Equal(AutoCompleteResults, result);
        await _primary.DidNotReceive()
            .GetAutoCompleteAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<double>(), Arg.Any<double>(),
                Arg.Any<double>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAutoComplete_CacheHit_DoesNotWriteToCache()
    {
        // Arrange
        _cache.GetAutoCompleteAsync("paris", "token", 0, 0, 0, "en", CancellationToken.None)
            .Returns(AutoCompleteResults);

        // Act
        await _placeService.GetAutoComplete("paris", "token", 0, 0, 0, "en", "", CancellationToken.None);

        // Assert
        await _cache.DidNotReceive()
            .SetAutoCompleteAsync(Arg.Any<IEnumerable<AutoComplete>>(), Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAutoComplete_CacheMiss_PrimaryReturnsResults_StoresAndReturnsResults()
    {
        // Arrange
        _cache.GetAutoCompleteAsync("paris", "token", 0, 0, 0, "en", CancellationToken.None)
            .Returns((IEnumerable<AutoComplete>?)null);
        _primary.GetAutoCompleteAsync("paris", "token", 0, 0, 0, "en", CancellationToken.None)
            .Returns(AutoCompleteResults);

        // Act
        var result = await _placeService.GetAutoComplete("paris", "token", 0, 0, 0, "en", "",
            CancellationToken.None);

        // Assert
        Assert.Equal(AutoCompleteResults, result);
        await _cache.Received(1)
            .SetAutoCompleteAsync(Arg.Any<IEnumerable<AutoComplete>>(), "paris", "en", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAutoComplete_CacheAndPrimaryMiss_SecondaryReturnsResults_StoresAndReturnsResults()
    {
        // Arrange
        _cache.GetAutoCompleteAsync("paris", "token", 0, 0, 0, "en", CancellationToken.None)
            .Returns((IEnumerable<AutoComplete>?)null);
        _primary.GetAutoCompleteAsync("paris", "token", 0, 0, 0, "en", CancellationToken.None)
            .Returns((IEnumerable<AutoComplete>?)null);
        _secondary.GetAutoCompleteAsync("paris", "token", 0, 0, 0, "en", CancellationToken.None)
            .Returns(AutoCompleteResults);

        // Act
        var result = await _placeService.GetAutoComplete("paris", "token", 0, 0, 0, "en", "",
            CancellationToken.None);

        // Assert
        Assert.Equal(AutoCompleteResults, result);
        await _cache.Received(1)
            .SetAutoCompleteAsync(Arg.Any<IEnumerable<AutoComplete>>(), "paris", "en", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAutoComplete_PrimaryThrows_FallsBackToSecondary()
    {
        // Arrange
        _cache.GetAutoCompleteAsync("paris", "token", 0, 0, 0, "en", CancellationToken.None)
            .Returns((IEnumerable<AutoComplete>?)null);
        _primary.GetAutoCompleteAsync("paris", "token", 0, 0, 0, "en", CancellationToken.None)
            .Returns(Task.FromException<IEnumerable<AutoComplete>?>(new HttpRequestException()));
        _secondary.GetAutoCompleteAsync("paris", "token", 0, 0, 0, "en", CancellationToken.None)
            .Returns(AutoCompleteResults);

        // Act
        var result = await _placeService.GetAutoComplete("paris", "token", 0, 0, 0, "en", "",
            CancellationToken.None);

        // Assert
        Assert.Equal(AutoCompleteResults, result);
    }

    [Fact]
    public async Task GetAutoComplete_AllProvidersMiss_ReturnsNull()
    {
        // Arrange
        _cache.GetAutoCompleteAsync("paris", "token", 0, 0, 0, "en", CancellationToken.None)
            .Returns((IEnumerable<AutoComplete>?)null);
        _primary.GetAutoCompleteAsync("paris", "token", 0, 0, 0, "en", CancellationToken.None)
            .Returns((IEnumerable<AutoComplete>?)null);
        _secondary.GetAutoCompleteAsync("paris", "token", 0, 0, 0, "en", CancellationToken.None)
            .Returns((IEnumerable<AutoComplete>?)null);

        // Act
        var result = await _placeService.GetAutoComplete("paris", "token", 0, 0, 0, "en", "",
            CancellationToken.None);

        // Assert
        Assert.Null(result);
        await _cache.DidNotReceive()
            .SetAutoCompleteAsync(Arg.Any<IEnumerable<AutoComplete>>(), Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<CancellationToken>());
    }
}